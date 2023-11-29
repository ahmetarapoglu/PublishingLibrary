using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.AccountModels;
using BookShop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BookShop.Concreate
{
    public class AccountRepository<T>: IAccountRepository<T> where T : class
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public AccountRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task CreateUser(User user, string password , string domain)
        {
            try
            {
                // Search by username
                var userByUsername = await _userManager.FindByNameAsync(user.UserName!);

                // If not found by username, search by email
                var userByEmail = await _userManager.FindByEmailAsync(user.Email!);

                if (userByUsername != null || userByEmail != null)
                    throw new OzelException(ErrorProvider.NotValid);

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                    throw new OzelException(ErrorProvider.DataNotFound);
                
                if(!user.EmailConfirmed)
                {
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var url = $"{domain}/app?userId={user.Id}&{emailConfirmationToken}";

                    var emailBody = $"Please confirm your email by clicking <a href='{url}'>here</a>.";

                    await _emailService.SendEmailAsync(user.Email!, "Email Confirmation", emailBody);
                }

            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);          
            }   
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var userById = await _userManager.FindByIdAsync(userId)
                    ?? throw new OzelException(ErrorProvider.DataNotFound);

                var result = await _userManager.ConfirmEmailAsync(userById!, token);

                if (!result.Succeeded)
                    throw new OzelException(ErrorProvider.DataNotFound);

            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            } 
        }
        public async Task IsEmailConfirmedAsync(User user)
        {
            try
            {
                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ForgetPasswordAsync(string email ,string domain)
        {
            try
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(email)
                         ?? throw new OzelException(ErrorProvider.NotValid);

                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                if (!isEmailConfirmed)
                    throw new OzelException(ErrorProvider.NotValid);

                // Generate a password reset token
                var passwordConfirmationToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var url = $"{domain}/app?userEmail={user.Email}&{passwordConfirmationToken}";

                var emailBody = $"Please reset your password by clicking <a href='{url}'>here</a>.";

                await _emailService.SendEmailAsync(user.Email!, "Password Reset", emailBody);

            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
        public async Task VerifyUserTokenAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                             ?? throw new OzelException(ErrorProvider.DataNotFound);

                var result = await _userManager.VerifyUserTokenAsync(
                            user,
                            _userManager.Options.Tokens.PasswordResetTokenProvider,
                            UserManager<User>.ResetPasswordTokenPurpose,
                            token);

                if (!result)
                    throw new OzelException(ErrorProvider.DataNotFound);
            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ResetPasswordAsync(string userId, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                         ?? throw new OzelException(ErrorProvider.DataNotFound);

               var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                //invalidToken
                if (!result.Succeeded)
                    throw new OzelException(ErrorProvider.DataNotFound);
            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ChangePasswordAsync(User user, string password, string newPassword)
        {
            try
            {
                var changePassword = await _userManager.ChangePasswordAsync(user, password, newPassword);

                if (!changePassword.Succeeded)
                    throw new OzelException(ErrorProvider.DataNotFound);
            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task UpdateProfileAsync(UpdateAccountModel model , User user)
        {
            try
            {
                var checkPassword = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                if (!checkPassword)
                    throw new OzelException(ErrorProvider.DataNotFound);

               var changePassword =  await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
               if (!changePassword.Succeeded)
                    throw new OzelException(ErrorProvider.DataNotFound);

                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Image = model.Image;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new OzelException(ErrorProvider.DataNotFound);
            }
            catch (OzelException ex)
            {
                throw new OzelException(ErrorProvider.NotValid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
