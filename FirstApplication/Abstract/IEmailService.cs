using BookShop.Models.EmailSender;

namespace BookShop.Abstract
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
