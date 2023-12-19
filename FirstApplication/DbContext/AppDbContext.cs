using BookShop.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static BookShop.Entities.Invoice;
using static BookShop.Entities.Order;

namespace BookShop.Db
{
    public class AppDbContext: IdentityDbContext<User, Role ,int>
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorAddress> AuthorAddresss { get; set; }
        public DbSet<AuthorBiography> AuthorBiyografies { get; set; }
        public DbSet<AuthorPayment> AuthorPayments { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookVersion> VersionBooks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchPayment> BranchPayments { get; set; }


        public DbSet<Category> Categories { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

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

            modelBuilder.ApplyConfiguration(new AuthorConfig());
            modelBuilder.ApplyConfiguration(new AuthorAddressConfig());
            modelBuilder.ApplyConfiguration(new AuthorBiographyConfig());
            modelBuilder.ApplyConfiguration(new AuthorPaymentConfig());

            modelBuilder.ApplyConfiguration(new BookVersionConfig());
            modelBuilder.ApplyConfiguration(new BranchPaymentConfig());
            modelBuilder.ApplyConfiguration(new BookAuthorConfig());
            modelBuilder.ApplyConfiguration(new BookCategoryConfig());
            modelBuilder.ApplyConfiguration(new BookAuthorConfig());
            modelBuilder.ApplyConfiguration(new BookConfig());

            modelBuilder.ApplyConfiguration(new CategoryConfig());

            modelBuilder.ApplyConfiguration(new InvoiceConfig());

            modelBuilder.ApplyConfiguration(new OrderConfig());

            modelBuilder.ApplyConfiguration(new RoleConfig());

            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new UserRoleConfig());

        }

    }
}
