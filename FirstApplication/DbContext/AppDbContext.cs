using BookShop.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static BookShop.Entities.Invoice;
using static BookShop.Entities.Order;

namespace BookShop.Db
{
    public class AppDbContext: IdentityDbContext<User, Role ,int>
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorAddress> AuthorAddresss { get; set; }
        public DbSet<AuthorBiography> AuthorBiyografis { get; set; }
        public DbSet<AuthorPayment> AuthorPayments { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookVersion> VersionBooks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchPayment> BranchPayments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.UseCollation("Turkish_CI_AS");

            modelBuilder.ApplyConfiguration(new UserRoleConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new RoleConfig());
            modelBuilder.ApplyConfiguration(new InvoiceConfig());

            //------------------------//
            //One To One RelationShip :
            //------------------------//

            modelBuilder.Entity<Author>()
                .HasOne<AuthorAddress>(s => s.AuthorAddress)
                .WithOne(ad => ad.Author)
                .HasForeignKey<AuthorAddress>(ad => ad.AuthorId);

            modelBuilder.Entity<Author>()
                .HasOne<AuthorBiography>(s => s.AuthorBiography)
                .WithOne(ad => ad.Author)
                .HasForeignKey<AuthorBiography>(ad => ad.AuthorId);

            //------------------------//
            //One To Many RelationShip :
            //------------------------//

            // Author - AuthorPayment
            modelBuilder.Entity<AuthorPayment>()
                .HasOne(s => s.Author)
                .WithMany(g => g.AuthorPayments)
                .HasForeignKey(s => s.AuthorId);

            // Category - Book
            modelBuilder.Entity<Book>()
                .HasOne(s => s.Category)
                .WithMany(g => g.Books)
                .HasForeignKey(s => s.CategoryId);

            // Book - BookVersion
            modelBuilder.Entity<BookVersion>()
                .HasOne(s => s.Book)
                .WithMany(g => g.BookVersions)
                .HasForeignKey(s => s.BookId);

            // Branch - BranchPayment
            modelBuilder.Entity<BranchPayment>()
                .HasOne(s => s.Branch)
                .WithMany(g => g.BranchPayments)
                .HasForeignKey(s => s.BranchId);

            //------------------------//
            //Many To Many RelationShip :
            //------------------------//

            // Book - Author
            modelBuilder.Entity<BookAuthor>()
                .HasOne(s => s.Book)
                .WithMany(g => g.BookAuthors)
                .HasForeignKey(s => s.BookId);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(s => s.Author)
                .WithMany(g => g.BookAuthors)
                .HasForeignKey(s => s.AuthorId);

            // BookVersion - Branch
            modelBuilder.Entity<Order>()
                .HasOne(s => s.BookVersion)
                .WithMany(g => g.Orders)
                .HasForeignKey(s => s.BookVersionId);

            modelBuilder.Entity<Order>()
                .HasOne(s => s.Branch)
                .WithMany(g => g.Orders)
                .HasForeignKey(s => s.BranchId);


            modelBuilder.Entity<BookAuthor>().HasKey(sc => new { sc.AuthorId, sc.BookId });
            modelBuilder.Entity<Order>().HasKey(sc => new { sc.Id });

        }

    }
}
