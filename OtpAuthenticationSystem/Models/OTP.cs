namespace OtpAuthenticationSystem.Models
{
    public class Otp
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Purpose { get; set; } = string.Empty;

        public string OtpHash { get; set; } = string.Empty;

        public string Salt { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime ExpireAt { get; set; }

        public bool IsUsed { get; set; }
    }
}