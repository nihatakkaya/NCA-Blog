//mevcut kategoriyi güncellerken gelen veri 
using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Categories
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}