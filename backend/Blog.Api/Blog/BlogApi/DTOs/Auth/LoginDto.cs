//kullanıcı giriş yaparken frontendden hangi bilgileri alacağımızı belirliyor. 

using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Auth
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
