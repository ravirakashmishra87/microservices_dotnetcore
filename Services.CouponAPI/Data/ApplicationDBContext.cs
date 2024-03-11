using Microsoft.EntityFrameworkCore;
using Services.CouponAPI.Models;

namespace Services.CouponAPI.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }

        public DbSet<Coupon> Coupons{ get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponID=1,
                CouponCode="101C",
                DiscountAmount=10,
                MinimumAmount=50,
            });
            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponID = 2,
                CouponCode = "102C",
                DiscountAmount = 20,
                MinimumAmount = 100,
            });
        }


    }
}
