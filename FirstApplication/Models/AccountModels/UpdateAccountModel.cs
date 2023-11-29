namespace BookShop.Models.AccountModels
{
    public class UpdateAccountModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Image { get; set; }
    }
}
