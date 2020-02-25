using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;

namespace ModelLayer
{
    public class CrmContext : IdentityDbContext<User, Role, long>
    {
        public CrmContext(DbContextOptions<CrmContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /**************** Renaming the tables from asp net *******************/
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
            });
            modelBuilder.Entity<IdentityUserRole<long>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<long>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<long>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<long>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<long>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
            /**********************************************************************/
        }

        //entities
        //public DbSet<Address> Addresses { get; set; }

        public DbSet<EducationalOpportunity> EducationalOpportunities { get; set; }
    }
}