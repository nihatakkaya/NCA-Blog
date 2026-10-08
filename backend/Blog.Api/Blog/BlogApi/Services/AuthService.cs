using BlogApi.Data;
using BlogApi.DTOs.Auth;
using BlogApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogApi.Exceptions;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;


namespace BlogApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context; //veritabanına ulaşmak için 
        private readonly IPasswordHasher<User> _passwordHasher; //şifreyi hashlemek ve kontrol etmek için
        private readonly IConfiguration _configuration; //ayar dosyalarındaki bilgileri okumak için jwt bilgisi gibi 

        public AuthService(
            AppDbContext context,   //veritabanına ulaşmak için
            IPasswordHasher<User> passwordHasher,   //şifreyi hashlemek ve kontrol etmek için
            IConfiguration configuration)   //ayar dosyalarındaki bilgileri okumak için jwt bilgisi gibi
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task RegisterAsync(RegisterDto dto)    //Kullanıcı kaydı yapar. Email daha önce var mı kontrol eder, şifreyi hashler, User oluşturur ve veritabanına kaydeder.
        {
            string normalizedEmail = dto.Email.Trim().ToLower();

            bool emailExists = await _context.Users
                .AnyAsync(x => x.Email.Trim().ToLower() == normalizedEmail);

            if (emailExists)
            {
                throw new BadRequestException("Bu email zaten kullanılıyor.");
            }

            var user = new User
            {
                Username = dto.Username.Trim(),
                Email = normalizedEmail
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)  //Kullanıcı girişini yapar. Email ile kullanıcıyı bulur, şifreyi kontrol eder.
        {
            string normalizedEmail = dto.Email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.Trim().ToLower() == normalizedEmail);

            if (user == null)
            {
                throw new UnauthorizedException("Email veya şifre hatalı.");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Email veya şifre hatalı.");
            }

            string accessToken = GenerateJwtToken(user);

            string refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = HashRefreshToken(refreshToken),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDto> RefreshAsync(RefreshTokenDto dto)    //refresh token kontrol edip üretme metodu.
        {
            string tokenHash = HashRefreshToken(dto.RefreshToken);  //frontend refesh token gönderiyor onu hash yapiyoruz

            var storedToken = await _context.RefreshTokens  //refresh token tablosuna gidiyoruz.
                .Include(x => x.User)   //refresh tokenın bağlı olduğu kullanıcıyı da çekiyoruz.
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);    //db deki hashle eşleşiyor mu diye bakıyoruz.

            if (storedToken == null)
            {
                throw new UnauthorizedException("Refresh token geçersiz.");
            }

            if (storedToken.RevokedAt != null)
            {
                throw new UnauthorizedException("Refresh token iptal edilmiş.");
            }

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh token süresi dolmuş.");
            }

            string newAccessToken = GenerateJwtToken(storedToken.User);

            string newRefreshToken = GenerateRefreshToken();

            storedToken.RevokedAt = DateTime.UtcNow;    //Bu refresh token şu anda iptal edildi anlamına geliyor.

            var newRefreshTokenEntity = new RefreshToken
            {
                TokenHash = HashRefreshToken(newRefreshToken),
                UserId = storedToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newRefreshTokenEntity);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> LogoutAsync(RefreshTokenDto dto)
        {
            string tokenHash = HashRefreshToken(dto.RefreshToken);

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

            if (storedToken == null)
            {
                return false;
            }

            if (storedToken.RevokedAt != null)
            {
                return false;
            }

            storedToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            return await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => new UserProfileDto
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Role = x.Role
                })
                .FirstOrDefaultAsync();
        }

        private string GenerateJwtToken(User user)  //Giriş yapan kullanıcı için JWT üretir.Token içine Id, Email, Role koyar, gizli key ile imzalar ve süre verir.
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),   //token 1 saat sonra geçersiz olsun demek 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()   //rastgele refresh token üretir.
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);    //64 byte rastgele veri üretir.

            return Convert.ToHexString(randomBytes);    //bu byteları okunabilir bir metne çevirir ve gerçek refresh token olarak döndürür.
        }

        private string HashRefreshToken(string refreshToken)    //dışarıdan refresh token alıyoruz.
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(refreshToken);   //refresh token metnini byte dizisine çevirir.

            byte[] hashBytes = SHA256.HashData(tokenBytes); //hash a dönüştürüyor.

            return Convert.ToHexString(hashBytes);  //hashi tekrar metne çevirip dönüştürür.
            //ilk olarak byte a çeviriyoruz çünkü has byte ile çalışır. sonrasında hash sonucunda byte[] çıkar onu da stringe dönüştürürüz çünkü db de metin olarak saklamak daha pratik.
        }
    }
}