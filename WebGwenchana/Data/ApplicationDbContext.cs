using Microsoft.EntityFrameworkCore;
using WebGwenchana.Models;
using WebGwenchana.ModelViews;


namespace WebGwenchana.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        { 
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<SizePrice> SizesPrice { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<TransactStatus> TransactStatuses { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<WebGwenchana.ModelViews.RegisterViewModel> RegisterViewModel { get; set; } = default!;
        public DbSet<WebGwenchana.ModelViews.ChangePasswordViewModel> ChangePasswordViewModel { get; set; } = default!;
    }
}
