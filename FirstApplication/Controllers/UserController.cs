using Azure.Core;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.UserModels;
using BookShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

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
            _context= context;
            _userManager= userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authenticationService.Login(request);

            return Ok(response);
        }

        //[AllowAnonymous]
        //[HttpPost("register")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        //{
        //    var response = await _authenticationService.Register(request);

        //    return Ok(response);
        //}

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.Select(i => new UserModel
                {
                    Id = i.Id,
                    UserName = i.UserName,
                    Email = i.Email
                }).ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetOneUser")]
        public async Task<IActionResult> GetOneUser(int id)
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
        [HttpGet("GetMyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                _ = int.TryParse(_userManager.GetUserId(User), out int userId);
                var data = await _context.Users.FirstOrDefaultAsync(i => i.Id == userId) ?? throw new Exception($"User whith this ID:{userId} Nof Found!.");
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
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserModel request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == request.Id) ?? throw new Exception($"User whith this ID:{request.Id} Nof Found!.");
                user.UserName = request.UserName;
                user.Email = request.Email;

                await _context.SaveChangesAsync();
                return Ok("User details updates");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteUser")]
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
