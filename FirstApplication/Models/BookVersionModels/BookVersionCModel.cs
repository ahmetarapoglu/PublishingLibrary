using BookShop.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.BookVersionModels
{
    public class BookVersionCModel : BookVersionModel
    {
        [Required(ErrorMessage = "BookId is required")]
        public int BookId { get; set; }
    }
}
