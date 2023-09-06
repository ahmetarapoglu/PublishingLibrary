using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.UserModels
{
    public class RegisterRequest
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
