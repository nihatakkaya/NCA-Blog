//API'ye yeni tag oluşturulurken gelen veriyi taşıyan sınıf.
using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Tags
{
    public class CreateTagDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}