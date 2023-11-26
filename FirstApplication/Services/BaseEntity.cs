namespace BookShop.Services
{
    public class BaseEntity : IEntity
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
