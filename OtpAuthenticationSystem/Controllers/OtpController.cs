using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using OtpAuthenticationSystem.Controllers;
using OtpAuthenticationSystem.Data;
using OtpAuthenticationSystem.Models;
using OtpAuthenticationSystem.Services;
using static System.Net.WebRequestMethods;

namespace OtpAuthenticationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OtpService _otpService;
        private readonly EmailService _emailServices;
        private readonly UserManager<IdentityUser> _userManager;

        public OtpController(ApplicationDbContext context , OtpService otpservice, EmailService emailservice, UserManager<IdentityUser> usermanager)
        {
            _context = context;
            _otpService = otpservice;
            _emailServices = emailservice;
            _userManager = usermanager;
        }
        [HttpPost("Send")]
        public async Task<IActionResult> SendOtp(SendOtpRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User Not Found"
                });
            }

            await _otpService.InvalidatePreviousOTP(
                user.Id,
                request.Purpose
            );

            string otp = _otpService.GenerateOtp();

            var result = _otpService.HashOtp(otp);

          var otpData = new Otp
          {
              UserId = user.Id,
              Email = request.Email,
              Purpose = request.Purpose,
              OtpHash = result.Hash,
              Salt = result.Salt,
              CreatedAt = DateTime.UtcNow,
              ExpireAt = _otpService.GetExpiryTime(),
              IsUsed = false
          };

            _context.otp.Add(otpData);

            await _context.SaveChangesAsync();

            string message =
                $"Your OTP is {otp}. This OTP is Valid For 5 Minutes.";

            await _emailServices.SendEmailAsync(
                request.Email,
                "Your OTP",
                message
            );

            return Ok(new
            {
                message = "OTP Sent successfully"
            });
        }

    }
}
