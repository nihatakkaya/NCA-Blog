//kullanıcı kayıt olurken frontend’den hangi bilgileri alacağımızı belirliyor.
using System.ComponentModel.DataAnnotations;
namespace BlogApi.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

    }
}
