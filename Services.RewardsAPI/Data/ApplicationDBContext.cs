using Microsoft.EntityFrameworkCore;

using Services.RewardsAPI.Models;

namespace Services.RewadsAPI.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }

        public DbSet<Rewards> Rewards{ get; set; }
        

       


    }
}
