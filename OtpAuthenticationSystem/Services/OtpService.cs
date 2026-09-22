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
    }
}
