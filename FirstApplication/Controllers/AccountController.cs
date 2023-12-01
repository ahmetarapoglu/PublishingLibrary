using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.AccountModels;
using BookShop.Models.UserModels;
using BookShop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly IAccountRepository<User> _accountRepository;


        public AccountController(
            UserManager<User> userManager,
            IRepository<User> userRepository,
            IAccountRepository<User> accountRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var token = await _accountRepository.Login(model);

            //Where
            Expression<Func<User, bool>> filter = i => i.Email == model.UserName
                                                    || i.UserName == model.UserName;

            //Select
            static IQueryable<AuthenticationUser> select(IQueryable<User> query) => query.Select(entity => new AuthenticationUser
            {
                Id = entity.Id,
                Email = entity.Email!,
                UserName = entity.UserName!,
                IsActive = entity.IsActive,
                Image = entity.Image,
                CreateDate = entity.CreateDate,
                RoleName = entity.UserRoles.Select(i => i.Role.Name).ToList()!,
            });

            var user = await _userRepository.FindAsync(select, filter);

            return Ok(new {token ,user});
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get the currently authenticated user
                var user = await _userManager.GetUserAsync(User)
                    ?? throw new OzelException(ErrorProvider.DataNotFound);

                await _accountRepository.Logout(user);

                return Ok();
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
