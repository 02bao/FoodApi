using FoodApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApi.Data
{
    public class FoodDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set;}

        //public static string configSql = "Host=192.168.28.81:5432;Database=se214;Username=postgres;Password=postgres";
        public static string configSql = "Host=localhost:5432;Database=se214;Username=postgres;Password=postgres";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(configSql);
        }

    }
}
