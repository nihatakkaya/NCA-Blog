using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Comments
{
    public class UpdateCommentDto
    {
        [Required]  //bu alanın boş geçilmemesi gerektiğini söylüyoruz.
        public string Content { get; set; } = string.Empty; //güncellenmiş yorum içeriği için property oluşturuyoruz.
    }
}