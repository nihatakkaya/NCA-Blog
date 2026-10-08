using BlogApi.Data;
using BlogApi.DTOs.Categories;
using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;
using BlogApi.Exceptions;

namespace BlogApi.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context; //yani .net ten gelen db bağlantısını service içinde kullanmak için.
        }

        //task list category işlem bittiğinde bir category listesi döndürecek demek 
        public async Task<Category> CreateCategoryAsync(CreateCategoryDto dto)
        {
            string normalizedName = dto.Name.Trim().ToLower();

            bool categoryExists = await _context.Categories
                .AnyAsync(category =>
                    category.Name.ToLower() == normalizedName);

            if (categoryExists)
            {
                throw new BadRequestException("Bu kategori zaten mevcut.");
            }

            var category = new Category //yeni category entity oluşturuyoruz. yani db ye kaydedilecek olan nesneyi oluşturuyoruz.
            {
                Name = dto.Name.Trim() //dto.nameapiden gelen değer, category.name db'ye kaydedilecek değer.
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .ToListAsync();
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)  //mevcut kategorinin adını değiştirmek için
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return false;
            }

            string normalizedName = dto.Name.Trim().ToLower();

            bool categoryExists = await _context.Categories
                .AnyAsync(existingCategory =>
                    existingCategory.Id != id &&
                    existingCategory.Name.ToLower() == normalizedName);

            if (categoryExists)
            {
                throw new BadRequestException("Bu kategori zaten mevcut.");
            }

            category.Name = dto.Name.Trim();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return false;
            }

            var posts = await _context.Posts
                .Where(post => post.CategoryId == id)
                .ToListAsync();

            foreach (var post in posts)
            {
                post.CategoryId = null;
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}