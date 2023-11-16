using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace BookShop.Concreate
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage message = new();

            message.From = new MailAddress("bookshopvip@gmail.com");
            message.To.Add(email);
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;

            SmtpClient smtp = new("smtp.gmail.com")
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("bookshopvip@gmail.com", "Aa-123456789"),
                //TargetName = "STARTTLS/smtp.gmail.com"
            };

            return smtp.SendMailAsync(message);
        }
    }
}
