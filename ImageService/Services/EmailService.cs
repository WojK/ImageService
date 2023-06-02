using System.Security.Authentication;
using ImagesService.Models.DTO;
using ImagesService.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ImagesService.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }


        public void SendEmail(EmailDTO emailDTO)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("mcompanyacc7@gmail.com"));
            email.To.Add(MailboxAddress.Parse(emailDTO.To));
            email.Subject = emailDTO.Subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDTO.Body
            };

            var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.CheckCertificateRevocation = false;

            smtp.Connect(_config.GetSection("EmailHost").Value, 587, MailKit.Security.SecureSocketOptions.StartTls);
            string? emailPassword = Environment.GetEnvironmentVariable("companyEmailPassword");
            smtp.Authenticate(_config.GetSection("EmailAddress").Value, emailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
