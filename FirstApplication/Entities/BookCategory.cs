using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class BookCategory
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

    public class BookCategoryConfig : IEntityTypeConfiguration<BookCategory>
    {
        public void Configure(EntityTypeBuilder<BookCategory> builder)
        {
            //------------------------//
            //Many To Many RelationShip :
            //------------------------//

            // Book - Author
            builder.HasKey(sc => new { sc.CategoryId, sc.BookId });

            builder.HasOne(s => s.Book)
                   .WithMany(g => g.BookCategories)
                   .HasForeignKey(s => s.BookId);

            builder.HasOne(s => s.Category)
                   .WithMany(g => g.BookCategories)
                   .HasForeignKey(s => s.CategoryId);

        }
    }
}
