using Microsoft.EntityFrameworkCore;
using Services.ShoppingCartAPI.Models;

namespace Services.ShoppingCartAPI.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }

        public DbSet<CartHeader> CartHeaders{ get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

       


    }
}
