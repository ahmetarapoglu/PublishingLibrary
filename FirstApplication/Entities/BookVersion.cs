using BookShop.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class BookVersion : BaseEntity
    {
        public int Number { get; set; }
        public int BookCount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal ProfitTotal { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public List<Order> Orders { get; set; }
    }
    public class BookVersionConfig : IEntityTypeConfiguration<BookVersion>
    {
        public void Configure(EntityTypeBuilder<BookVersion> builder)
        {

            //------------------------//
            // One To Many RelationShip :
            //------------------------//

            // Book - BookVersion
             builder.HasOne(s => s.Book)
                    .WithMany(g => g.BookVersions)
                    .HasForeignKey(s => s.BookId);
        }
    }
}
