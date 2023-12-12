using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class AuthorAddress
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }


        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

    public class AuthorAddressConfig : IEntityTypeConfiguration<AuthorAddress>
    {
        public void Configure(EntityTypeBuilder<AuthorAddress> builder)
        {

            //------------------------//
            // One To One RelationShip :
            //------------------------//

            // Author - AuthorAddress
            builder.HasOne(s => s.Author)
                    .WithOne(ad => ad.AuthorAddress)
                    .HasForeignKey<AuthorAddress>(ad => ad.AuthorId);
        }
    }
}
