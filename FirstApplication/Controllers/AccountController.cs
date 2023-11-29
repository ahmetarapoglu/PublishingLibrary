using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.AccountModels;
using BookShop.Models.UserModels;
using BookShop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<User> _userManager;
        private readonly IAccountRepository<User> _accountRepository;


        public AccountController(
            UserManager<User> userManager,
            IAuthenticationService authenticationService,
            IAccountRepository<User> accountRepository)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
            _accountRepository = accountRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var response = await _authenticationService.Login(request);
            var data = await _userManager.FindByNameAsync(request.UserName);

            if (data is null)
            {
                data = await _userManager.FindByEmailAsync(request.UserName);
            }

            var user = new UserRModel
            {
                Id = data.Id,
                UserName = data.UserName,
                Email = data.Email
            };

            return Ok(new { token = response, user });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SignUp(RegisterModel model)
        {
            try
            {
                var user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = false,
                };

                var domain = HttpContext.Request.Scheme + "//" + HttpContext.Request.Host.Value;

                await _accountRepository.CreateUser(user, model.Password, domain);

                return Ok();
            }

            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmEmailAsync(string id, string token)
        {
            try
            {
                await _accountRepository.ConfirmEmailAsync(id, token);

                return Ok(ErrorProvider.Success);
            }

            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ForgetPasswordAsync(string email)
        {
            try
            {
                var domain = HttpContext.Request.Scheme + "//" + HttpContext.Request.Host.Value;

                await _accountRepository.ForgetPasswordAsync(email, domain);

                return Ok(ErrorProvider.Success);
            }

            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> VerifyUserTokenAsync(string userId, string token)
        {
            try
            {
                await _accountRepository.VerifyUserTokenAsync(userId, token);

                return Ok(ErrorProvider.Success);
            }

            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ResetPasswordAsync(string email, string token, string password, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email)
                    ?? throw new OzelException(ErrorProvider.DataNotFound);

                var userId = user.Id.ToString();

                await _accountRepository.ResetPasswordAsync(userId, token, newPassword);

                return Ok(ErrorProvider.Success);
            }

            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangePasswordAsync(string password, string newPassword)
        {
            try
            {
                // Get the currently authenticated user
                var user = await _userManager.GetUserAsync(User)
                    ?? throw new OzelException(ErrorProvider.DataNotFound);

                await _accountRepository.ChangePasswordAsync(user, password, newPassword);

                return Ok(ErrorProvider.Success);
            }

            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateProfileAsync(UpdateAccountModel model)
        {
            try
            {
                // Get the currently authenticated user
                var user = await _userManager.GetUserAsync(User)
                    ?? throw new OzelException(ErrorProvider.DataNotFound);

                await _accountRepository.UpdateProfileAsync(model, user);

                return Ok(ErrorProvider.Success);
            }

            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                // Get the currently authenticated user
                var user = await _userManager.GetUserAsync(User)
                    ?? throw new OzelException(ErrorProvider.DataNotFound);

                var profile = new UserRModel
                {
                    Id = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!,
                    IsActive = user.IsActive!,
                    Image = user.Image!,
                };

                return Ok(profile);
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
