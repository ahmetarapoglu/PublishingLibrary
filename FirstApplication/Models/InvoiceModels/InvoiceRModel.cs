using BookShop.Models.OrderModels;

namespace BookShop.Models.InvoiceModels
{
    public class InvoiceRModel
    {
        public int Id { get; set; }
        public bool IsInvoiced { get; set; }
        public OrderRModel Order { get; set; }

    }
}
