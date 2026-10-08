using Microsoft.EntityFrameworkCore;
using BlogApi.Entities;


namespace BlogApi.Data
{
    public class AppDbContext : DbContext   //veritabanıyla  konuşabilen sınıf haline getiriyoruz.
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) //program.cs te verdiğimiz postgresql ayarlarını alıyoruz 
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}