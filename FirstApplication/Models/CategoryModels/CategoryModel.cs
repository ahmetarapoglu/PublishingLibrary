
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.CategoryModels
{
    public class CategoryModel
    {
        [Required(ErrorMessage = "CategoryName is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "CategoryName must be between 3 and 50 characters")]
        public string CategoryName { get; set; }
    }
}
