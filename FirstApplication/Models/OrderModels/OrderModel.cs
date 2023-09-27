
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.OrderModels
{
    public class OrderModel
    {
        [Required(ErrorMessage = "BranchId is required")]
        public int BranchId { get; set; }
        [Required(ErrorMessage = "BranchId is required")]
        public int BookVersionId { get; set; }
        [Required(ErrorMessage = "BranchId is required")]
        public int BookCount { get; set; }
    }
}
