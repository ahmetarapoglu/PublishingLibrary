namespace BookShop.Models.BookVersionModels
{
    public class BookVersionRModel : BookVersionModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public decimal TotalCostPrice { get; set; }
        public decimal TotalSellPrice { get; set; }

    }
}
