//bu clas veritabanından aldığımız post bilgisinin frontende hangi haliyle döneceğini belirler

namespace BlogApi.DTOs.Posts
{
    public class PostResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string AuthorName { get; set; } = string.Empty;

        public string? CategoryName { get; set; }
        public List<string> Tags { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}