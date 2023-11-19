using BookShop.Models.EmailSender;

namespace BookShop.Abstract
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest model);
    }
}
