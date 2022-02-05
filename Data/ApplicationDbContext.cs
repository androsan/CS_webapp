using CS_webapp.Models;
using Microsoft.EntityFrameworkCore;

namespace CS_webapp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    public DbSet<Category> Category { get; set; }

    }

}
