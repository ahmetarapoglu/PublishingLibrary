using BookShop.Models.BookModels;
using BookShop.Models.BookVersionModels;

namespace BookShop.Models.AuthorModels
{
    public class BookInAuthors
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CategoryName { get; set; }
        public List<BookVersionRModel> BookVersions { get; set; }
    }
}
