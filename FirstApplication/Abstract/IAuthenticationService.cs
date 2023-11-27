using BookShop.Models.AccountModels;

namespace BookShop.Abstract
{
    public interface IAuthenticationService
    {
        Task<string> Register(RegisterModel request);
        Task<string> Login(LoginModel request);

    }
}
