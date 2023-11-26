using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AccountModels;
using BookShop.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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


        public AccountController(
            UserManager<User> userManager,
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<IActionResult> GetUserProfile()
        //{
        //    try
        //    {
        //        _ = int.TryParse(_userManager.GetUserId(User), out int userId);

        //        var data = await _context.Users.FirstOrDefaultAsync(i => i.Id == userId)
        //            ?? throw new Exception($"User whith this ID:{userId} Nof Found!.");

        //        var user = new UserModel
        //        {
        //            Id = data.Id,
        //            Email = data.Email,
        //            UserName = data.UserName,
        //            IsActive = data.IsActive,
        //            Image = data.Image,
        //        };

        //        return Ok(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
