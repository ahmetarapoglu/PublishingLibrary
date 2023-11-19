using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Entities
{
    public class User: IdentityUser<int>
    {
        //public bool IsLocked { get; set; }
        //public string Image { get; set; }
        public List<UserRole> UserRoles { get; set; }

    }
    class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

        }
    }

}

