using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Tags
{
    public class UpdateTagDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}