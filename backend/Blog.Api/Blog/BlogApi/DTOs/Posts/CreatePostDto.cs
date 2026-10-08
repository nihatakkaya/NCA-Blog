//bu forntendden gelen veri 
using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Posts
{
    public class CreatePostDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}