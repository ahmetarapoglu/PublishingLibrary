namespace BookShop.Models.InvoiceModels
{
    public class InvoiceUModel: InvoiceModel
    {
        public int Id { get; set; }
        public int BookVersionId { get; set; }
        public int BookCount { get; set; }
    }
}
