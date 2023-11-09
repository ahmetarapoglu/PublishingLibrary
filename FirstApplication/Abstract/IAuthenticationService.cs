using BookShop.Models.UserModels;

namespace BookShop.Abstract
{
    public interface IAuthenticationService
    {
        Task<string> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);

    }
}
