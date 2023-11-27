using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.AccountModels
{
    public class LoginModel
    {
        [DefaultValue("admin@test.com")]
        public string? UserName { get; set; }
        [DefaultValue("Admin.123")]
        public string? Password { get; set; }
    }
}
