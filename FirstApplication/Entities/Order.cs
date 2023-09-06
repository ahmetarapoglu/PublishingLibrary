namespace BookShop.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int BookCount { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public int BookVersionId { get; set; }
        public BookVersion BookVersion { get; set; }
    }
}
