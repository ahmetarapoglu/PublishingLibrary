using BookShop.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public List<BookCategory> BookCategories { get; set; }

    }
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
        }
    }
}
