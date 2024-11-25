using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebSite.Identity.Statics;

namespace WebSite.Identity.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = UserRoles.User,
                    NormalizedName = UserRoles.NormalizedUser,
                },
                new IdentityRole
                {
                    Name = UserRoles.Admin,
                    NormalizedName = UserRoles.NormalizedAdmin,
                }
            );
        }
    }
}
