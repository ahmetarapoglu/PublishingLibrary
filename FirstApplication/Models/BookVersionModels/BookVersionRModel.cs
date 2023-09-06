namespace BookShop.Models.BookVersionModels
{
    public class BookVersionRModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int BookCount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TotalCostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalSellPrice { get; set; }
        public int LibraryRatio { get; set; }

    }
}
