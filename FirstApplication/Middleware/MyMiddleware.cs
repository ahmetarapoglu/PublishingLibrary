using BookShop.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookShop.Middleware
{
    public class MyMiddleware
    {
        private readonly RequestDelegate _next;

        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager)
        {
            // Extract the token from the request headers
            var token = context.Request.Headers["Authorization"];

            var currentUserClaims = context.User.Claims.ToList();

            var customClaimValue = "";

            if (currentUserClaims.Count > 0)
                customClaimValue = currentUserClaims.FirstOrDefault(i => i.Type == "jti")!.Value;

            var userByName = context.User.Identity!.Name;

            var securityStamp = "";

            if (userByName != null)
                securityStamp = userManager.Users.FirstOrDefault(i => i.UserName == userByName)!.SecurityStamp;

            if (securityStamp != customClaimValue)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _next(context);
        }
    }

}
