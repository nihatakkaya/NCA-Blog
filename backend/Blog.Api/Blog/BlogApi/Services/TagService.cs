using BlogApi.Data;
using BlogApi.DTOs.Tags;
using BlogApi.Entities;
using BlogApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class TagService
    {
        private readonly AppDbContext _context;

        public TagService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> CreateTagAsync(CreateTagDto dto)
        {
            string normalizedName = dto.Name.Trim().ToLower();

            bool tagExists = await _context.Tags
                .AnyAsync(tag =>
                    tag.Name.ToLower() == normalizedName);

            if (tagExists)
            {
                throw new BadRequestException("Bu tag zaten mevcut.");
            }

            var tag = new Tag
            {
                Name = dto.Name.Trim()
            };

            _context.Tags.Add(tag);

            await _context.SaveChangesAsync();

            return tag;
        }
        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _context.Tags
                .ToListAsync();
        }

        public async Task<bool> UpdateTagAsync(int id, UpdateTagDto dto)    //güncellenecek tag id ve dto alır
        {
            var tag = await _context.Tags.FindAsync(id);    //id ile tag bulur

            if (tag == null)
                return false;

            tag.Name = dto.Name;    //db den bulduğumuz tagın adı ve kullanıcının gönderdiği isim eşitlenir

            await _context.SaveChangesAsync();  //db ye kaydetme değişikliği

            return true;
        }
        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);    //silmek istediğimiz tagı id ile db de buluyoruz.

            if (tag == null)
                return false;

            _context.Tags.Remove(tag);

            await _context.SaveChangesAsync();  //bulduğumuz tag ı silinmek üzere işaretliyoruz.

            return true;
        }
    }
}