using BookShop.Entities;
using BookShop.Models.OrderModels;

namespace BookShop.Models.InvoiceModels
{
    public class InvoiceCModel: InvoiceModel
    {
        public static Func<InvoiceCModel, Invoice> Fill => model => new Invoice
        {
             OrderId = model.OrderId,
             IsInvoiced = model.IsInvoiced,
        };
    }
}
