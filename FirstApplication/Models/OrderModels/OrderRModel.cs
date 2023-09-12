namespace BookShop.Models.OrderModels
{
    public class OrderRModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int BookVersionId { get; set; }
        public int BookCount { get; set; }
        public decimal Total { get; set; }
        public decimal profitTotal { get; set; }
    }
}
