//frontende yorumlar hangi bilgilerle döndürülecek

namespace BlogApi.DTOs.Comments
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}