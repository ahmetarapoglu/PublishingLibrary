using System.Net;
using System.Net.Mail;
using BookShop.Abstract;
using BookShop.Models.EmailSender;
using Microsoft.Extensions.Options;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient
        {
            Host = _emailSettings.MailServer,
            Port = _emailSettings.MailPort,
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password),
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }

}

