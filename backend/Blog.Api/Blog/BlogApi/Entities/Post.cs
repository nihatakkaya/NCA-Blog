namespace BlogApi.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty; //özet

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;   //yazar
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public List<Tag> Tags { get; set; } = new();    //bir post birden fazla tag içerebilir

        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;//oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; }
    }
}
