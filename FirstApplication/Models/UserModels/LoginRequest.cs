using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.UserModels
{
    public class LoginRequest
    {
        [DefaultValue("admin@test.com")]
        public string? UserName { get; set; }
        [DefaultValue("Admin.123")]
        public string? Password { get; set; }
    }
}
