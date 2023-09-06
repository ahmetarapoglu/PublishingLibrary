namespace BookShop.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<BookAuthor> BookAuthors { get; set;}
        public List<BookVersion> BookVersions { get; set;}
    }
}
