using Microsoft.EntityFrameworkCore;
using Services.EmailAPI.Models;

namespace Services.EmailAPI.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }

        public DbSet<EmailLogger> EmailLoggers{ get; set; }

        


    }
}
