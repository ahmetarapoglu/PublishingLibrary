using BookShop.Entities;
using BookShop.Models.BookVersionModels;

namespace BookShop.Models.BookModels
{
    public class BookRModel :BookModel
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public List<BookCategoryModel> Categories { get; set; }
        public List<BookVersionRModel> BookVersions { get; set; }
    }

}
