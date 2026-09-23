using Microsoft.EntityFrameworkCore;
using OtpAuthenticationSystem.Data;
using System.Security.Cryptography;
namespace OtpAuthenticationSystem.Services
{
    public class OtpService
    {
        private readonly ApplicationDbContext _context;

        public OtpService(ApplicationDbContext context)
        {
            _context = context;
        } 
        public string GenerateOtp()
        {
            int otp = RandomNumberGenerator.GetInt32(100000, 1000000);

            return otp.ToString();
        }
        public (string Hash,string Salt) HashOtp(string otp)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                otp, salt, 100000, HashAlgorithmName.SHA256,
                32
                );
            return (
                Convert.ToBase64String(hash),
                Convert.ToBase64String(salt)
                );
        }
        public DateTime GetExpiryTime()
        {
            return DateTime.UtcNow.AddMinutes(5);
        }

        public async Task InvalidatePreviousOTP(string UserId, string Purpose)
        {
            var previousOtps = await _context.otp
                .Where(x => x.UserId == UserId &&
                            x.Purpose == Purpose &&
                            !x.IsUsed
                ).ToListAsync();

            foreach(var otp in previousOtps)
            {
                otp.IsUsed = true;
            }

        }
        public async Task<bool> IsCooldownActive(string userId, string purpose)
        {
            var lastOtp = await _context.otp
                .Where(x => x.UserId == userId &&
                x.Purpose == purpose)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if(lastOtp == null)
            {
                return false;
            }
            var cooldownTime = lastOtp.CreatedAt.AddSeconds(60);
            return DateTime.UtcNow < cooldownTime;
        }

        public async Task<bool> IsRateLimitExceeded(string userId, string purpose)
        {
            var windowstart = DateTime.UtcNow.AddMinutes(-10);

            var requestCount = await _context.otp
                .CountAsync(x => x.UserId == userId &&
                 x.Purpose == purpose &&
                 x.CreatedAt >= windowstart);

            return requestCount >= 5;
        }

        public bool VerifyOtp(string otp,string storedHash,string storedSalt)
        {
            byte[] salt = Convert.FromBase64String(storedSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(otp, salt, 100000, HashAlgorithmName.SHA256, 32);

            string enterOtpHash = Convert.ToBase64String(hash);
            return enterOtpHash == storedHash;
        }
    }
}
