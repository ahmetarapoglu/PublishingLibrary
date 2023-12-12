namespace BookShop.Models.BookModels
{
    public class BookUModel : BookModel
    {
        public int Id { get; set; }
        public List<int> CategoriesId { get; set; }
    }
}
