using System;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;

namespace ModelLayer
{
    public class CrmContext : DbContext
    {
        public CrmContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=CRMDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        //entities
        public DbSet<Address> Addresses{ get; set; }
    }
}
