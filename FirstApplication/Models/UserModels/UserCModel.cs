namespace BookShop.Models.UserModels
{
    public class UserCModel : UserModel
    {
        public List<int> UserRoles { get; set; }
        public string Password { get; set; }
    }
}
