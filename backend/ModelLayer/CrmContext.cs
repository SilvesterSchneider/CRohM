using System;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using ModelLayer.Models.Base;

namespace ModelLayer
{
    public class CrmContext : DbContext
    {
        public CrmContext(DbContextOptions<CrmContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(user => user.Name)
                .HasMaxLength(2);
        }

        //entities
        public DbSet<Address> Addresses { get; set; }

        public DbSet<User> Users { get; set; }
    }

    public class User : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}