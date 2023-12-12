
using BookShop.Services;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.CategoryModels
{
    public class CategoryModel
    {
        [Required(ErrorMessage = "CategoryName is required")]
        public string CategoryName { get; set; }
    }
}
