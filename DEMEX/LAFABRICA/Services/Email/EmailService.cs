using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace LAFABRICA.Services.Email
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        
        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task sendEmail(string to,  string subject, string body)
        {
            var settings = _config.GetSection("EmailSettings");

            using var smtp = new SmtpClient(settings["Host"], int.Parse(settings["Port"]!))
            {
                Credentials = new NetworkCredential(settings["FromEmail"], settings["Password"]),
                EnableSsl = bool.Parse(settings["EnableSsl"]!)
            };

            var message = new MailMessage {
                From = new MailAddress(settings["FromEmail"]!, settings["FromName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true   
            };

            message.To.Add(to);

            await smtp.SendMailAsync(message);
        }
    }
}
