using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.CategoryModels;
using BookShop.Models.RequestModels;
using BookShop.Models.UserModels;
using BookShop.Services;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;


        public UserController(
            UserManager<User> userManager,
            IRepository<User> userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetUsers(UserRequest model)
        {
            try
            {

                //Where.
                Expression<Func<User, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.UserName.Contains(model.Search));

                //Sort.
                Expression<Func<User, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "userName" => i => i.UserName,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<User> orderBy(IQueryable<User> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select
                static IQueryable<UserRModel> select(IQueryable<User> query) => query.Select(entity => new UserRModel
                {
                    Id = entity.Id,
                    Email = entity.Email,
                    UserName = entity.UserName,
                    RoleName = entity.UserRoles.Select(i=>i.Role.Name).ToList(),
                    IsActive = entity.IsActive,
                    Image = entity.Image,
                    CreateDate = entity.CreateDate,
                });

                var (total, data) = await _userRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

                return Ok(new { data, total });

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
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                //Where
                Expression<Func<User, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<UserRModel> select(IQueryable<User> query) => query.Select(entity => new UserRModel
                {
                    Id = entity.Id,
                    Email = entity.Email,
                    UserName = entity.UserName,
                    RoleName = entity.UserRoles.Select(i => i.Role.Name).ToList(),
                    IsActive = entity.IsActive,
                    Image = entity.Image,
                    CreateDate= entity.CreateDate,
                });

                var category = await _userRepository.FindAsync(select, filter);

                return Ok(category);
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

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(UserCModel model)
        {
            try
            {
                // Search by username
                var userByUsername = await _userManager.FindByNameAsync(model.UserName);
                                        
                // If not found by username, search by email
                var userByEmail = await _userManager.FindByEmailAsync(model.Email);

                if (userByUsername != null || userByEmail != null)
                    throw new OzelException(ErrorProvider.NotValid);

                var user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    IsActive = model.IsActive,
                    Image = model.Image,
                    CreateDate = DateTime.Now,
                    EmailConfirmed = true,
                };

                await _userManager!.CreateAsync(user, model.Password);

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
        public async Task<IActionResult> UpdateUser(UserUModel model)
        {
            try
            {
                if (model.Id == 0 || model.Id == null)
                    throw new Exception("Reauested User Not Found!.");


                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //Where
                Expression<Func<User, bool>> filter = i => i.Id == model.Id;

                var entity = await _userRepository.FindAsync(filter);

                entity!.UserName = model.UserName;
                entity.Email = model.Email;
                entity.IsActive = model.IsActive;
                entity.Image = model.Image;
                entity.UserRoles = model.UserRoles.Select(i => new UserRole
                {
                    RoleId = i
                }).ToList();    

                await _userRepository.UpdateAsync(entity);

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
        public async Task<IActionResult> ChangeUserState(int userId , bool isActive)
        {
            try
            {
                if (userId == 0 || userId == null)
                    throw new Exception("Reauested User Not Found!.");


                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //Where
                Expression<Func<User, bool>> filter = i => i.Id == userId;

                var entity = await _userRepository.FindAsync(filter);

                entity!.IsActive = isActive;

                await _userRepository.UpdateAsync(entity);

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

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                //Where
                Expression<Func<User, bool>> filter = i => i.Id == id;

                await _userRepository.DeleteAsync(filter);

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
    }
}
