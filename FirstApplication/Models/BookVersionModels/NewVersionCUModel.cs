using BookShop.Entities;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace BookShop.Models.BookVersionModels
{
    public class NewVersionCUModel
    {
        [Required(ErrorMessage = "BookId is required")]
        public int BookId { get; set; }
        [Required(ErrorMessage = "BookCount is required")]
        public int BookCount { get; set; }
        [Required(ErrorMessage = "CostPrice is required")]
        public decimal CostPrice { get; set; }
        [Required(ErrorMessage = "SellPrice is required")]
        public decimal SellPrice { get; set; }
        [Required(ErrorMessage = "LibraryRatio is required")]
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
