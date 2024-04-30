using Microsoft.EntityFrameworkCore;
using Services.OrderAPI.Models;

namespace Services.OrderAPI.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }

        public DbSet<OrderMaster> OrderMaster{ get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

       


    }
}
