namespace BookShop.Services
{
    public interface IEntity
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
