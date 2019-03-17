using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Restaurant_page.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_page
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CheckoutCustomer> CheckoutCustomers { get; set; }
        public DbSet<CheckoutBasket> Baskets { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<CheckoutBasketItem> BasketItems { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        [NotMapped]
        public DbSet<CheckoutItem> CheckoutItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CheckoutBasketItem>().HasKey(t => new { t.MenuID, t.BasketID });
            builder.Entity<OrderItem>().HasKey(t => new { t.MenuID, t.OrderNo });
        }

    }
}
