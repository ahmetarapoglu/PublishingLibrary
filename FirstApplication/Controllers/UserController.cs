using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.RequestModels;
using BookShop.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;


        public UserController(AppDbContext context, UserManager<User> userManager, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authenticationService.Login(request);
            var data = await _userManager.FindByNameAsync(request.UserName);

            if (data is null)
            {
                data = await _userManager.FindByEmailAsync(request.UserName);
            }

             var user = new UserModel { 
                 Id = data.Id,
                 UserName = data.UserName,
                 Email = data.Email
             };
            return Ok(new { token = response , user });
        }

        [HttpPost("AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddUser([FromBody] RegisterRequest request)
        {
            var response = await _authenticationService.Register(request);

            return Ok(new { status = true });

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUsers(UserRequest model)
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var data =users
                    .Where(i => i.UserName.Contains(model.Search))
                    .OrderByDescending(i => i.Id)
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .Select(i => new UserModel
                    {
                        Id = i.Id,
                        UserName = i.UserName,
                        Email = i.Email
                    });
                return Ok(new { total= users.Count, data });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var data = await _context.Users.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"User whith this ID:{id} Nof Found!.");
                var user = new UserModel
                {
                    Id = data.Id,
                    Email = data.Email,
                    UserName = data.UserName
                };
                return Ok(user);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                _ = int.TryParse(_userManager.GetUserId(User), out int userId);
                var data = await _context.Users.FirstOrDefaultAsync(i => i.Id == userId)
                    ?? throw new Exception($"User whith this ID:{userId} Nof Found!.");
                var user = new UserModel
                {
                    Id = data.Id,
                    Email = data.Email,
                    UserName = data.UserName
                };
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateUser(UserModel request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == request.Id) ?? throw new Exception($"User whith this ID:{request.Id} Nof Found!.");
                user.UserName = request.UserName;
                user.Email = request.Email;

                await _context.SaveChangesAsync();
                return Ok(new { status = true });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"User whith this ID:{id} Nof Found!.");
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok("User Removed Successfuly!.");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
