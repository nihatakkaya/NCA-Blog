using BlogApi.DTOs.Auth;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);

            return Ok(response);    //dto yu frontend e json olarak gönderiyor.
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
        {
            var response = await _authService.RefreshAsync(dto);

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenDto dto)
        {
            bool loggedOut = await _authService.LogoutAsync(dto);

            if (!loggedOut)
            {
                return BadRequest("Refresh token bulunamadı veya zaten iptal edilmiş.");
            }

            return Ok("Çıkış başarılı.");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!//jwt içindeki kullanıcı id sini alıyor.
            );

            var profile = await _authService.GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return Ok(profile);
        }

        [Authorize]     
        [HttpGet("test")]
        public IActionResult Test() //bu endpointe jwt olmadan girilmez gelen kişide geçerli jwt var mı test ediliyor.
        {
            return Ok("Token geçerli, giriş yapılmış.");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-test")]
        public IActionResult AdminTest()    //JWT geçerli mi + rol Admin mi kontrol etmek için deneme endpointi
        {
            return Ok("Admin yetkisi başarılı.");
        }
    }
}