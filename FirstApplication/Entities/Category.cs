using BookShop.Services;

namespace BookShop.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public List<Book> Books { get; set; }
    }
}
