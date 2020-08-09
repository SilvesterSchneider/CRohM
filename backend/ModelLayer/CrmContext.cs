using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Helper;
using ModelLayer.Models;

namespace ModelLayer
{
    public class CrmContext : IdentityDbContext<User, Permission, long>
    {
        public CrmContext(DbContextOptions<CrmContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TODO SeedPermission
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Permission>()
                .HasData(new Permission
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
            modelBuilder.Entity<Permission>()
            .HasData(new Permission
            {
                Id = 2,
                Name = "DeleteUser",
                NormalizedName = "DeleteUser".ToUpper()
            });

            modelBuilder.Entity<Permission>()
                .Property(role => role.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(user => user.Id)
                .ValueGeneratedOnAdd();

            /**************** Renaming the tables from asp net *******************/
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
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

            modelBuilder.Entity<OrganizationContact>().HasKey(sc => new { sc.OrganizationId, sc.ContactId });
            modelBuilder.Entity<OrganizationContact>()
                .HasOne(sc => sc.Contact)
                .WithMany(s => s.OrganizationContacts)
                .HasForeignKey(sc => sc.ContactId);

            modelBuilder.Entity<OrganizationContact>()
                .HasOne(sc => sc.Organization)
                .WithMany(s => s.OrganizationContacts)
                .HasForeignKey(sc => sc.OrganizationId);
            modelBuilder.Entity<EventContact>().HasKey(sc => new { sc.EventId, sc.ContactId });
            modelBuilder.Entity<EventContact>()
                .HasOne(sc => sc.Contact)
                .WithMany(s => s.Events)
                .HasForeignKey(sc => sc.ContactId);
            modelBuilder.Entity<EventContact>()
                .HasOne(sc => sc.Event)
                .WithMany(s => s.Contacts)
                .HasForeignKey(sc => sc.EventId);
            modelBuilder.Entity<UserPermissionGroup>().HasKey(sc => new { sc.UserId, sc.PermissionGroupId });
            modelBuilder.Entity<UserPermissionGroup>()
                .HasOne(sc => sc.User)
                .WithMany(s => s.Permission)
                .HasForeignKey(sc => sc.UserId);
            modelBuilder.Entity<UserPermissionGroup>()
                .HasOne(sc => sc.PermissionGroup)
                .WithMany(s => s.User)
                .HasForeignKey(sc => sc.PermissionGroupId);
        }

        //entities

        public DbSet<Address> Addresses { get; set; }
        public DbSet<OrganizationContact> OrganizationContacts { get; set; }
        public DbSet<EducationalOpportunity> EducationalOpportunities { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<EventContact> EventContacts { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Participated> Participations { get; set; }
        public DbSet<HistoryElement> History { get; set; }
        public DbSet<ModificationEntry> ModificatonHistory { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<UserLogin> UserLogin { get; set; }
        public DbSet<UserPermissionGroup> UserPermissionGroups { get; set; }
    }
}
