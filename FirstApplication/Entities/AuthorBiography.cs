using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class AuthorBiography
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NativeLanguage { get; set; }
        public string Education { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
    public class AuthorBiographyConfig : IEntityTypeConfiguration<AuthorBiography>
    {
        public void Configure(EntityTypeBuilder<AuthorBiography> builder)
        {

            //------------------------//
            // One To One RelationShip :
            //------------------------//

            // Author - AuthorBiography
            builder.HasOne(s => s.Author)
                    .WithOne(ad => ad.AuthorBiography)
                    .HasForeignKey<AuthorBiography>(ad => ad.AuthorId);
        }
    }
}
