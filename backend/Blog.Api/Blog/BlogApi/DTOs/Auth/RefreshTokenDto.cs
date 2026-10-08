//refresh token kullanarak access token üretme endpointi 
using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;    //frontendten gelen refresh tokenı tutmak için.
    }
}