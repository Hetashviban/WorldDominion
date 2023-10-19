using Microsoft.EntityFrameworkCore;
//Used for migrations and creates DB context 

namespace WorldDominion.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Change to be your model(s) and table(s)
        public DbSet<Department> Departments { get; set; }
        //The above line is the association between the department model and the department table

        public DbSet<Product> Products { get; set; }
    }
}