using BookShop.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }


        public int BookId { get; set; }
        public Book Book { get; set; }
    }
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //------------------------//
            // One To Many RelationShip :
            //------------------------//
            builder.HasKey(c => c.Id);

            // Book - Category
            builder.HasOne(s => s.Book)
                   .WithMany(g => g.Categories)
                   .HasForeignKey(s => s.BookId)
                   .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed
        }
    }
}
