using BlogApi.Data;
using BlogApi.DTOs.Comments;
using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class CommentService
    {
        private readonly AppDbContext _context; //veritabanına ulaşabilmek için 

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        //async bu metodun içinde bekleme süresi olan işler yapacağımızı söyler.
        //await ise bu işlem bitene kadar sonucu bekle anlamına geliyor.
        public async Task<Comment> CreateCommentAsync(  //burada bilgileri alıyoruz 
            CreateCommentDto dto,
            int userId,
            int postId)
        {
            var comment = new Comment   //comment entity oluşturuyoruz
            {
                Content = dto.Content,
                UserId = userId,
                PostId = postId
            };

            _context.Comments.Add(comment); //yorumu comments tablosuna ekliyoruz
            await _context.SaveChangesAsync();  //postgrasql e kaydediyoruz 

            return comment;
        }

        public async Task<List<CommentResponseDto>> GetCommentsByPostIdAsync(int postId)
        {   //postId numarasına göre blog yorumu getirme.
            return await _context.Comments  //veritabanındaki comments tablosuna erişiyoruz
                .Where(c => c.PostId == postId) //postId numarasına göre filtreleme yapıyoruz
                .Select(c => new CommentResponseDto //veritabanından gelen comment entitylerini frontende dönecek
                {
                    Id = c.Id,  //yorumun id'sini alıyoruz
                    Content = c.Content,    //yorumun içeriğini alıyoruz
                    Username = c.User.Username, //yorumun sahibinin kullanıcı adını alıyoruz
                    CreatedAt = c.CreatedAt //yorumun oluşturulma tarihini alıyoruz
                })
                .ToListAsync(); //bulduğumuz bütün yorumları liste haline getiriyoruz ve frontende döndürüyoruz
        }

        public async Task<bool> UpdateCommentAsync(
            int commentId,  //hangi yorum güncellenecek
            int userId, //hangi kullanıcı güncelleme yapacak
            UpdateCommentDto dto)//yeni yorum içeriği
        {
            var comment = await _context.Comments.FindAsync(commentId); //comments tablosunda commentId numarasına göre yorum buluyoruz

            if (comment == null)    //yorum var mı yoksa false yorum bu kullanıcıya mı ait değilse false contenti değiştir savechangesasync yap.
            {
                return false;
            }

            if (comment.UserId != userId)
            {
                return false;
            }

            comment.Content = dto.Content;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)//commentid hangi yorum silinecek userid silmeye çalışan kullanıcı kim
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
                return false;

            if (comment.UserId != userId)
                return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}