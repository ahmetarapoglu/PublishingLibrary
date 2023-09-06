using BookShop.Entities;

namespace BookShop.Models.BookVersionModels
{
    public class NewVersionCUModel
    {
        public int BookId { get; set; }
        public int BookCount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int LibraryRatio { get; set; }
        public static Func<NewVersionCUModel,int, BookVersion> Fill => (model , number) =>
        new BookVersion
        {
            BookId = model.BookId,
            Number = number + 1,
            BookCount = model.BookCount,
            CostPrice = model.CostPrice,
            SellPrice = model.SellPrice,
            LibraryRatio = model.LibraryRatio
        };         
    }
}
