using Microsoft.EntityFrameworkCore;
using Services.ProductAPI.Models;

namespace Services.ProductAPI.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }

        public DbSet<Product> Products{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product
            {
               ProductId = 1,
               Name = "Test",
               Description = "This is a test data seed",
               Price=10.5,
               CategoryName="Test",
               ImageUrl="https://placehold.co/603x403"
            });            
        }


    }
}
