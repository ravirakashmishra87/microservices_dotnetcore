using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Services.AuthAPI.Models;

namespace Services.AuthAPI.Data
{
    public class ApplicationDBContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }

       
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);
        }


    }
}
