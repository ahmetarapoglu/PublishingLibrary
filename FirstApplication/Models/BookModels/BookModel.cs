using BookShop.Models.BookVersionModels;

namespace BookShop.Models.BookModels
{
    public class BookModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<AuthorInBookModel> BookAuthors { get; set; }
        public BookVersionCUModel BookVersions { get; set; }
    }
}
