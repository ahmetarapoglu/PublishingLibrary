using BookShop.Services;

namespace BookShop.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Cover { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<BookAuthor> BookAuthors { get; set;}

        public List<BookVersion> BookVersions { get; set;}
    }
}
