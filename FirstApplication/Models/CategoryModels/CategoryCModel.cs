using BookShop.Entities;

namespace BookShop.Models.CategoryModels
{
    public class CategoryCModel : CategoryModel
    {

        public static Func<CategoryCModel, Category> Fill => model => new Category
        {
            CategoryName = model.CategoryName,
        };
    }
}
