using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class AuthorPayment
    {
        public int Id { get; set; }
        public int PaymentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
    public class AuthorPaymentConfig : IEntityTypeConfiguration<AuthorPayment>
    {
        public void Configure(EntityTypeBuilder<AuthorPayment> builder)
        {

            //------------------------//
            // One To Many RelationShip :
            //------------------------//

            // Author - AuthorPayment
            builder.HasOne(s => s.Author)
                   .WithMany(g => g.AuthorPayments)
                   .HasForeignKey(s => s.AuthorId);
        }
    }
}
