using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.BookModels
{
    public class BookModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime PublishedDate { get; set; }
        public string Cover { get; set; }
        public List<AuthorInBookModel> BookAuthors { get; set; }
    }
}
