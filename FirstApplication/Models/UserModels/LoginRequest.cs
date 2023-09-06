using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.UserModels
{
    public class LoginRequest
    {
        [Required]
        [DefaultValue("admin@test.com")]
        public string? UserName { get; set; }
        [Required]
        [DefaultValue("Admin.123")]
        public string? Password { get; set; }
    }
}
