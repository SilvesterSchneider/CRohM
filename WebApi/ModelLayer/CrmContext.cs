using System;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;

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
        }

        //entities
        public DbSet<Address> Addresses { get; set; }
    }
}