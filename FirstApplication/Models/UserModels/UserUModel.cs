using BookShop.Entities;

namespace BookShop.Models.UserModels
{
    public class UserUModel : UserModel
    {
        public int Id { get; set; }
        public List<int> UserRoles { get; set; }

    }
}
