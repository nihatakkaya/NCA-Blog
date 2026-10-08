using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Comments
{
    public class CreateCommentDto
    {
        [Required]
        public string Content { get; set; } = string.Empty; //sadece content alıyoruz çünkü userid jwt den, ğpstid urlden , content frontendden gelecek.
    }
}