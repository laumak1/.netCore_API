using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saitynai.Models;

namespace Saitynai.Models
{
    public class SaitynaiContext : IdentityDbContext<User>
    {
       public SaitynaiContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region "Seed Data"

            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new { Id = "2", Name = "Customer", NormalizedName = "CUSTOMER" },
                new { Id = "3", Name = "Guest", NormalizedName = "GUEST" }
            );

            #endregion
        }


        //public DbSet<Saitynai.Models.User> Users { get; set; }
        public DbSet<Saitynai.Models.Order> Orders { get; set; }
        public DbSet<Saitynai.Models.OrderProduct> OrderProducts { get; set; }
        public DbSet<Saitynai.Models.Product> Products { get; set; }
    }
}
