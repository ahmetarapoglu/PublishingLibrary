using BookShop.Models.BookVersionModels;

namespace BookShop.Models.BookModels
{
    public class BookRModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CategoryName { get; set; }
        public List<AuthorInBookModel> BookAuthors { get; set; }
        public List<BookVersionRModel> BookVersions { get; set; }

    }
}
