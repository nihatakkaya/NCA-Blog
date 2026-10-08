//hem accesstoken hem de refreshtoken döndürmek için bu classı oluşturuyoruz.
//burda oluşturmamızın sebebi de token giriş işleminin bir parçası olduğu için 
namespace BlogApi.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}