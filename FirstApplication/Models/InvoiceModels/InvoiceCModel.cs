using BookShop.Entities;

namespace BookShop.Models.InvoiceModels
{
    public class InvoiceCModel : InvoiceModel
    {
        public int BookVersionId { get; set; }
        public int BookCount { get; set; }
    }
}
