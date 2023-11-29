using BookShop.Entities;
using BookShop.Models.AccountModels;

namespace BookShop.Abstract
{
    public interface IAccountRepository<T> where T : class
    {
        Task CreateUser(User user, string password, string domain);
        Task ConfirmEmailAsync(string userId, string token);
        Task IsEmailConfirmedAsync(User user);
        Task ForgetPasswordAsync(string email, string domain);
        Task VerifyUserTokenAsync(string userId, string token);
        Task ResetPasswordAsync(string userId, string token, string newPassword);
        Task ChangePasswordAsync(User user, string password, string newPassword);
        Task UpdateProfileAsync(UpdateAccountModel model, User user);
    }
}
