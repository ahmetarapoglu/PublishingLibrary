using BookShop.Entities;

namespace BookShop.Models.CategoryModels
{
    public class CategoryCUModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        public static Func<CategoryCUModel, Category> Fill => model => new Category
        {
            CategoryName = model.CategoryName,
        };
    }
}
