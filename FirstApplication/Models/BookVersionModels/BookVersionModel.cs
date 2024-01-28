
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.BookVersionModels
{
    public class BookVersionModel
    {
        [Required(ErrorMessage = "BookId is required")]
        public int BookCount { get; set; }
        [Required(ErrorMessage = "CostPrice is required")]
        public decimal CostPrice { get; set; }
        [Required(ErrorMessage = "SellPrice is required")]
        public decimal SellPrice { get; set; }
    }
}
