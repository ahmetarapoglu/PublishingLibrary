using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.RequestModels;
using BookShop.Models.UserModels;
using BookShop.Services;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IAccountRepository<User> _accountRepository;
        private readonly AppDbContext _context;


        public UserController(
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IAccountRepository<User> accountRepository,
            IRepository<Role> roleRepository,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
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
                    filter = filter.And(i => i.UserName!.Contains(model.Search));

                //Sort.
                Expression<Func<User, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "userName" => i => i.UserName!,
                    "email" => i => i.Email!,
                    "date" => i => i.CreateDate,
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
                    Email = entity.Email!,
                    UserName = entity.UserName!,
                    RoleName = entity.UserRoles.Select(i=>i.Role.Name).ToList()!,
                    IsActive = entity.IsActive!,
                    Image = entity.Image!,
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
                    Email = entity.Email!,
                    UserName = entity.UserName!,
                    RoleName = entity.UserRoles.Select(i => i.Role.Name).ToList()!,
                    IsActive = entity.IsActive,
                    Image = entity.Image!,
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddUser(UserCModel model)
        {
            try
            {
                var user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    IsActive = model.IsActive,
                    Image = model.Image,
                    CreateDate = DateTime.Now,
                    EmailConfirmed = true,
                    UserRoles = model.UserRoles.Select(i => new UserRole
                    {
                        RoleId = i
                    }).ToList()
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
        public async Task<IActionResult> UpdateUser(UserUModel model)
        {
            try
            {
                if (model?.Id == 0 || model?.Id == null)
                    throw new Exception("Reauested User Not Found!.");

                //// Retrieve the user with roles
                //var user = await _context.Users
                //     .Include(u => u.UserRoles)
                //     .FirstOrDefaultAsync(u => u.Id == model.Id);

                //// Update user properties
                //user.UserName = model.UserName;
                //user.Email = model.Email;
                //user.IsActive = model.IsActive;
                //user.Image = model.Image;

                //// Remove existing roles
                ////_context.UserRoles.RemoveRange(user.UserRoles);

                //// Add new roles
                //user.UserRoles = model.UserRoles.Select(roleId => new UserRole { 
                //    //UserId = user.Id,
                //    RoleId = roleId
                //}).ToList();

                //// Save changes to the database
                //await _context.SaveChangesAsync();

                //Where
                Expression<Func<UserRole, bool>> filter = i => i.UserId == model.Id;

                await _userRoleRepository.DeleteRangeAsync(filter);

                //Include.
                IIncludableQueryable<User, object> include(IQueryable<User> query) => query
                   .Include(i => i.UserRoles).ThenInclude(i=>i.Role);

                void action(User entity)
                {
                    entity!.UserName = model.UserName;
                    entity.Email = model.Email;
                    entity.IsActive = model.IsActive;
                    entity.Image = model.Image;
                    entity.UserRoles = model.UserRoles.Select(i => new UserRole
                    {
                        RoleId = i
                    }).ToList();
                }

                await _userRepository.UpdateAsync(action, i => i.Id == model.Id, include);


                // Find user by Id
                //var user = await _userManager.FindByIdAsync(model.Id.ToString())
                //       ?? throw new OzelException(ErrorProvider.DataNotFound);

                //user!.UserName = model.UserName;
                //user.Email = model.Email;
                //user.IsActive = model.IsActive;
                //user.Image = model.Image;
                //user.UserRoles = model.UserRoles.Select(i => new UserRole
                //{
                //    RoleId = i
                //}).ToList();

                //await _userManager.UpdateAsync(user);

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

                //Where
                Expression<Func<User, bool>> filter = i => i.Id == userId;

                void action(User user)
                {
                    user!.IsActive = isActive;
                }

                await _userRepository.UpdateAsync(action, filter);

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


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetRoles(UserRequest model)
        {
            try
            {

                //Where.
                Expression<Func<Role, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.Name!.Contains(model.Search));

                //Sort.
                Expression<Func<Role, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "roleName" => i => i.Name!,
                    "date" => i => i.CreateDate,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Role> orderBy(IQueryable<Role> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select
                static IQueryable<RoleRModel> select(IQueryable<Role> query) => query.Select(entity => new RoleRModel
                {
                    Id = entity.Id,
                    RoleName = entity.Name,
                    CreateDate = entity.CreateDate,
                });

                var (total, data) = await _roleRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

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
    }
}
