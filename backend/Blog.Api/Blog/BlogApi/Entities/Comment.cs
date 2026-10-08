namespace BlogApi.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
