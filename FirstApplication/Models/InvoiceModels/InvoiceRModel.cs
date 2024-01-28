using BookShop.Models.OrderModels;

namespace BookShop.Models.InvoiceModels
{
    public class InvoiceRModel
    {
        public int Id { get; set; }
        //public int OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public OrderRModel Order { get; set; }

    }
}
