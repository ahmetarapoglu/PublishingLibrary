using BookShop.Models.BookVersionModels;

namespace BookShop.Models.BookModels
{
    public class BookRModel :BookModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public List<BookVersionRModel> BookVersions { get; set; }

    }

}
