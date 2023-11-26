using BookShop.Db;
using BookShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static BookShop.Services.Global;

namespace BookShop.Seed
{
    public class DbInitializer
    {
        public static async Task InitializerAsync(IApplicationBuilder app)
        {
			try
			{
				using var serviceScope = app.ApplicationServices.CreateScope();
				var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

				var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
				var _roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

				if (context == null) return;

				await context.Database.EnsureCreatedAsync();

				//Create Role Function.
				async Task<Role> CreateRoleAsync(string roleName,bool isSystemRole = false)
				{
					var role = await _roleManager.Roles.FirstOrDefaultAsync(i => i.Name == roleName);
			
					if(role is null)
					{
						role = new Role 
						{
							Name = roleName,
						};

                        await _roleManager.CreateAsync(role);
					}
                    return role;
				}

				//Create User Function.
				async Task CreateUserAsync (string userName , string email , string password ="Admin.123" ,params int[]? roleIds)
				{
					if (!await context.Users.AnyAsync(i=>i.Email == email))
					{
						var user = new User()
						{
							UserName = userName,
							Email = email,
							EmailConfirmed = true,
							UserRoles = roleIds?.Select(i =>
								new UserRole
								{
									RoleId = i
								}
							).ToList()!
                        };

						await _userManager!.CreateAsync(user , password);
					}
				}

				//Create Role.
				var adminRole = await CreateRoleAsync(SystemRoles.Admin);
				var userRole = await CreateRoleAsync(SystemRoles.User);

                //Create the user whith a password.
                await CreateUserAsync("Admin", "admin@test.com", "Admin.123", adminRole.Id);
				await context.SaveChangesAsync();

            }
			catch (Exception ex)
			{

				throw;
			}
        }
    }
}
