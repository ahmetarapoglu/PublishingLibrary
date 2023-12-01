namespace BookShop.Models.AccountModels
{
    public class AuthenticationUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public List<string> RoleName { get; set; }
    }
}
