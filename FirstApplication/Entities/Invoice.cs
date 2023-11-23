using BookShop.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Entities
{
    public class Invoice : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public class InvoiceConfig : IEntityTypeConfiguration<Invoice>
        {
            public void Configure(EntityTypeBuilder<Invoice> builder)
            {

                //------------------------//
                //One To One RelationShip :
                //------------------------//

                //Order - Invoice.
                builder.HasOne<Order>(i => i.Order)
                        .WithOne(o => o.Invoice)
                        .HasForeignKey<Invoice>(i => i.OrderId);
            }
        }
    }
}
