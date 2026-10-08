using BlogApi.Data;
using BlogApi.DTOs.Posts;
using BlogApi.Entities;
using BlogApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class PostService
    {
        private readonly AppDbContext _context; //post tablosuna ulaşabilmek için vt bağlantısı tutma

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PostResponseDto> CreatePostAsync(CreatePostDto dto, int authorId)    //Yeni blog oluşturur ve veritabanına kaydeder.
        {
            if (dto.CategoryId.HasValue)    //kullanıcı gerçekten category seçti mi kontrolü için.
            {
                bool categoryExists = await _context.Categories
                    .AnyAsync(category => category.Id == dto.CategoryId.Value);

                if (!categoryExists)
                {
                    throw new NotFoundException("Kategori bulunamadı.");
                }
            }

            //kullanıcının yazdığı tagleri temizliyoruz.
            var tagNames = dto.Tags
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            //büyük küçük harf farkını kaldırarak arama yapabilmek için tag isimlerini küçültüyoruz.
            var normalizedTagNames = tagNames
                .Select(name => name.ToLowerInvariant())
                .ToList();

            //kullanıcının yazdığı taglerden daha önce veritabanında olanları buluyoruz.
            var tags = await _context.Tags
                .Where(tag => normalizedTagNames.Contains(tag.Name.ToLower()))
                .ToListAsync();

            //veritabanında olmayan tagleri oluşturuyoruz.
            foreach (var tagName in tagNames)
            {
                bool tagExists = tags.Any(tag =>
                    string.Equals(tag.Name, tagName, StringComparison.OrdinalIgnoreCase));

                if (!tagExists)
                {
                    var newTag = new Tag
                    {
                        Name = tagName
                    };

                    tags.Add(newTag);
                }
            }

            var post = new Post
            {
                Title = dto.Title,      //bunlar dto daki title post tittle a dto content post content e dçnüşüyor
                Content = dto.Content,  //yani kısaca dto dan gelen veriyi vtna kaydedilecek post entitysine çeviriyoruz.
                Summary = dto.Summary,
                AuthorId = authorId,
                CategoryId = dto.CategoryId,
                Tags = tags
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return await _context.Posts
                .Where(savedPost => savedPost.Id == post.Id)
                .Select(savedPost => new PostResponseDto
                {
                    Id = savedPost.Id,
                    Title = savedPost.Title,
                    Content = savedPost.Content,
                    Summary = savedPost.Summary,
                    AuthorName = savedPost.Author.Username,
                    CategoryName = savedPost.Category != null ? savedPost.Category.Name : null,
                    Tags = savedPost.Tags
                        .Select(tag => tag.Name)
                        .ToList(),
                    CreatedAt = savedPost.CreatedAt
                })
                .FirstAsync();
        }

        public async Task<PagedPostResponseDto> GetPostsAsync(int page, int pageSize)    //post tablosuna gidip blogları alıp her postu postresponsedto ya çeviriyor. liste halinde geri döndürüyor.
        {
            var totalCount = await _context.Posts   //toplam kaç tane yayınlanmış post olduğunu bulmak için
                .Where(post => post.IsPublished)
                .CountAsync();

            var posts = await _context.Posts
                .Where(post => post.IsPublished)    //post tablosundaki postları dolaş, ispublished true olanları al.
                .OrderByDescending(post => post.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(post => new PostResponseDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    AuthorName = post.Author.Username,
                    CategoryName = post.Category != null ? post.Category.Name : null,
                    Tags = post.Tags
                        .Select(tag => tag.Name)
                        .ToList(),
                    CreatedAt = post.CreatedAt
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);  //toplam sayfa sayısını hesaplıyor.

            return new PagedPostResponseDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = posts
            };
        }

        public async Task<List<PostResponseDto>> SearchPostsAsync(string query)
        {
            return await _context.Posts
                .Where(post => post.IsPublished &&  //taslak postların çıkmaması için 
                    (
                        EF.Functions.ILike(post.Title, $"%{query}%") || //postgre sql de büyük küçük harf önemsemeden arama yaptırıyor.
                        EF.Functions.ILike(post.Content, $"%{query}%") ||
                        EF.Functions.ILike(post.Summary, $"%{query}%")
                    ))
                .Select(post => new PostResponseDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    AuthorName = post.Author.Username,
                    CategoryName = post.Category != null ? post.Category.Name : null,
                    Tags = post.Tags
                        .Select(tag => tag.Name)
                        .ToList(),
                    CreatedAt = post.CreatedAt
                })
                .ToListAsync();
        }


        public async Task<List<PostResponseDto>> GetAllPostsForAdminAsync()
        {
            return await _context.Posts
                .Select(post => new PostResponseDto //post tablosu verisi post responsedto şekline çeviriliyor ve liste halinde geri dönüyor.
                {
                    Id = post.Id,   //postresponsedto içindeki veriyi vtnından gelen değere koyma işlemi yapılıyor.
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    AuthorName = post.Author.Username,
                    CreatedAt = post.CreatedAt,
                    CategoryName = post.Category != null ? post.Category.Name : null,   //postun kategorisi varsa kategori adını categoryname içine koyuyor, yoksa null koyuyor.
                    Tags = post.Tags
                        .Select(tag => tag.Name)
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<PostResponseDto?> GetPostByIdAsync(int id)    //Verilen ID’ye sahip tek blog yazısını getirir.
        {
            return await _context.Posts
                .Where(post => post.Id == id && post.IsPublished)   //id doğru olacak ve true olacak
                .Select(post => new PostResponseDto
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Summary = post.Summary,
                    AuthorName = post.Author.Username,
                    CreatedAt = post.CreatedAt,
                    CategoryName = post.Category != null ? post.Category.Name : null,
                    Tags = post.Tags
                        .Select(tag => tag.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePostAsync(int id, UpdatePostDto dto)  //Var olan blog yazısını günceller.
        {
            var post = await _context.Posts
                .Include(post => post.Tags)
                .FirstOrDefaultAsync(post => post.Id == id);

            if (post == null)
            {
                return false;
            }

            if (dto.CategoryId.HasValue)
            {
                bool categoryExists = await _context.Categories
                    .AnyAsync(category => category.Id == dto.CategoryId.Value);

                if (!categoryExists)
                {
                    throw new NotFoundException("Kategori bulunamadı.");
                }
            }

            //kullanıcının güncellemede yazdığı tagleri temizliyoruz.
            var tagNames = dto.Tags
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            //büyük küçük harf farkını kaldırarak arama yapabilmek için tag isimlerini küçültüyoruz.
            var normalizedTagNames = tagNames
                .Select(name => name.ToLowerInvariant())
                .ToList();

            //güncellemede gönderilen taglerden veritabanında zaten olanları buluyoruz.
            var tags = await _context.Tags
                .Where(tag => normalizedTagNames.Contains(tag.Name.ToLower()))
                .ToListAsync();

            //güncellemede gönderilen taglerden veritabanında olmayanları oluşturuyoruz.
            foreach (var tagName in tagNames)
            {
                bool tagExists = tags.Any(tag =>
                    string.Equals(tag.Name, tagName, StringComparison.OrdinalIgnoreCase));

                if (!tagExists)
                {
                    var newTag = new Tag
                    {
                        Name = tagName
                    };

                    tags.Add(newTag);
                }
            }

            post.Title = dto.Title;
            post.Content = dto.Content;
            post.Summary = dto.Summary;
            post.CategoryId = dto.CategoryId;

            //postun eski tag bağlantılarını kaldırıyoruz.
            post.Tags.Clear();

            //kullanıcının güncellemede gönderdiği tagleri posta tekrar bağlıyoruz.
            foreach (var tag in tags)
            {
                post.Tags.Add(tag);
            }

            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePostAsync(int id)     //Blog yazısını siler.
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return false;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePublishStatusAsync(int id, bool isPublished)  //hangi blogun durumu değişecek true ise yayınla false ise taslağa al 
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return false;
            }

            post.IsPublished = isPublished; //postun numarası ile true gelirse yaynılanmış false gelirse taslak olarak işaretleniyor.
            post.UpdatedAt = DateTime.UtcNow;   //yayınlanma durumu değiştiğinde blogun son güncellenme tarihi de güncelleniyor.

            await _context.SaveChangesAsync();  //değişiklikler postgresql e kaydediliyor.

            return true;
        }

    }
}