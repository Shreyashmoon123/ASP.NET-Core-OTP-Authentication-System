using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using OtpAuthenticationSystem.Data;
using OtpAuthenticationSystem.Models;
using OtpAuthenticationSystem.Services;
using System.Text;

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

        public OtpController(
            ApplicationDbContext context,
            OtpService otpservice,
            EmailService emailservice,
            UserManager<IdentityUser> usermanager)
        {
            _context = context;
            _otpService = otpservice;
            _emailServices = emailservice;
            _userManager = usermanager;
        }

        [HttpPost("Send")]
        public async Task<IActionResult> SendOtp(SendOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid Email Address"
                });
            }

            var AllowedPurpose = new[]
            {
                "PasswordReset",
                "Login",
                "EmailVerification"
            };

            if (!AllowedPurpose.Contains(request.Purpose))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid OTP purpose"
                });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "User Not Found"
                });
            }

            var cooldownActive = await _otpService.IsCooldownActive(
                user.Id,
                request.Purpose
            );

            if (cooldownActive)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Please wait before requesting another OTP"
                });
            }

            var rateLimitExceeded = await _otpService.IsRateLimitExceeded(
                user.Id,
                request.Purpose
            );

            if (rateLimitExceeded)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "OTP Request Limit Exceeded. Please Try Again Later"
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

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "OTP Sent successfully"
            });
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
        {
            var otpData = await _context.otp
                .Where(x => x.Email == request.Email &&
                            x.Purpose == request.Purpose &&
                            !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpData == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "OTP Not Found"
                });
            }

            if (DateTime.UtcNow > otpData.ExpireAt)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Otp Has Expired"
                });
            }

            bool isValid = _otpService.VerifyOtp(
                request.OTP,
                otpData.OtpHash,
                otpData.Salt
            );

            if (!isValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid OTP"
                });
            }

            otpData.IsUsed = true;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "OTP Verified Successfully"
            });
        }
    }
}
