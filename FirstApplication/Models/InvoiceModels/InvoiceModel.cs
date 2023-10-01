

using System.ComponentModel;

namespace BookShop.Models.InvoiceModels
{
    public class InvoiceModel
    {
        public bool IsInvoiced { get; set; }
        public int OrderId { get; set; }
    }
}
