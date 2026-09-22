using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
namespace OtpAuthenticationSystem.Services
{
    public class EmailService 
    {

        public async Task SendEmailAsync(string email, string subject, string messageBody)
        {
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;
            string SenderEmail = "shreyashkatiyar404@gmail.com";
            string SenderPassword = "nocroenizmrcznll";

            using MailMessage message = new MailMessage();

            message.From = new MailAddress(SenderEmail);
            message.To.Add(email);
            message.Subject = subject;
            message.Body = messageBody;

            using SmtpClient smtpclient = new SmtpClient(smtpHost, smtpPort);

            smtpclient.Credentials = new NetworkCredential(SenderEmail, SenderPassword);

            smtpclient.EnableSsl = true;
            await smtpclient.SendMailAsync(message);
        }
    }
}
