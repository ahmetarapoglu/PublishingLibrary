
namespace BookShop.Models.InvoiceModels
{
    public class InvoiceCModel : InvoiceModel
    {
        public int OrderId { get; set; }
        public int BookVersionId { get; set; }
        public int BookCount { get; set; }
    }
}
