using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class BookAuthor
    {
        public int AuhorRatio { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

    }

    public class BookAuthorConfig : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            //------------------------//
            //Many To Many RelationShip :
            //------------------------//

            // Book - Author
            builder.HasKey(sc => new { sc.AuthorId, sc.BookId });

            builder.HasOne(s => s.Book)
                   .WithMany(g => g.BookAuthors)
                   .HasForeignKey(s => s.BookId);

            builder.HasOne(s => s.Author)
                   .WithMany(g => g.BookAuthors)
                   .HasForeignKey(s => s.AuthorId);

        }
    }
}
