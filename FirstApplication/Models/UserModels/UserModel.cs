using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace BookShop.Models.UserModels
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
