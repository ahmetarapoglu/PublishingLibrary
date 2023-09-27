using BookShop.Models.BookVersionModels;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.BookModels
{
    public class BookModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(17, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 17 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 50 characters")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PublishedDate { get; set; }
        public List<AuthorInBookModel> BookAuthors { get; set; }
        public BookVersionCUModel BookVersions { get; set; }
    }
}
