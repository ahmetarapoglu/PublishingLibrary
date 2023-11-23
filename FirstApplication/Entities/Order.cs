
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BookShop.Services;

namespace BookShop.Entities
{
    public class Order : BaseEntity
    {
        public int BranchId { get; set; }
        public int BookCount { get; set; }
        public bool IsInvoiced { get; set; }


        public Branch Branch { get; set; }

        public int BookVersionId { get; set; }
        public BookVersion BookVersion { get; set; }
        public Invoice Invoice { get; set; }

    }
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {


            builder.Property(o => o.IsInvoiced)
                   .HasDefaultValue(false);
        }
    }
}
