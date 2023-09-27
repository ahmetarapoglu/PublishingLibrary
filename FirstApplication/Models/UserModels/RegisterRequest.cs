using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BookShop.Models.UserModels
{
    public class RegisterRequest
    {
        [DefaultValue("UserName")]
        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(17, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 17 characters.")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }
        [DefaultValue("Admin.123")]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string? Password { get; set; }
    }
}
