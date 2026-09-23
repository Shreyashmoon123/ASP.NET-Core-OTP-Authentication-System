using System.ComponentModel.DataAnnotations;

namespace OtpAuthenticationSystem.Models
{
    public class SendOtpRequest
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }
}
