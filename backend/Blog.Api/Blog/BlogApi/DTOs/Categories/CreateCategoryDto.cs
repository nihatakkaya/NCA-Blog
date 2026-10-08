//yeni kategori oluştururken gelen veri.
using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}