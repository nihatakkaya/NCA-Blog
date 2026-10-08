namespace BlogApi.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string TokenHash { get; set; } = string.Empty;   //refresh token çok güçlü bir bilgi. bu yüzden kendisini değil tokenhash saklıyoruz.

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}