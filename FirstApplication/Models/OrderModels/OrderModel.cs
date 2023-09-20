namespace BookShop.Models.OrderModels
{
    public class OrderModel
    {
        public int BranchId { get; set; }
        public int BookVersionId { get; set; }
        public int BookCount { get; set; }
    }
}
