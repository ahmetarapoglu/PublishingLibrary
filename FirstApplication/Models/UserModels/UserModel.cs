using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace BookShop.Models.UserModels
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(17, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 17 characters.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
    }
}
