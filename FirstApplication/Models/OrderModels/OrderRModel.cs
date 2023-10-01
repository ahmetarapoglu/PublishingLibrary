namespace BookShop.Models.OrderModels
{
    public class OrderRModel : OrderModel
    {
        public int Id { get; set; }
        public bool IsInvoiced { get; set; }
        public decimal Total { get; set; }
        public decimal profitTotal { get; set; }

    }
}
