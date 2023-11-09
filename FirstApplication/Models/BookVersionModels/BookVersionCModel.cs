using BookShop.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.BookVersionModels
{
    public class BookVersionCModel : BookVersionModel
    {
        [Required(ErrorMessage = "BookId is required")]
        public int BookId { get; set; }

        public static Func<BookVersionCModel, int, BookVersion> Fill => (model, number) =>
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
