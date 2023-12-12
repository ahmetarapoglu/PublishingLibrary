using BookShop.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Entities
{
    public class Author : BaseEntity
    {
        public string NameSurname { get; set; }
        public string Image { get; set; }

        public AuthorAddress AuthorAddress { get; set; }
        public AuthorBiography AuthorBiography { get; set; }

        public List<BookAuthor> BookAuthors { get; set; }

        public List<AuthorPayment> AuthorPayments { get; set; }
    }
    public class AuthorConfig : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");
        }
    }
}
