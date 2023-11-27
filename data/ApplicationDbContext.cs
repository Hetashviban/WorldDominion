using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//Used for migrations and creates DB context 

namespace WorldDominion.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Change to be your model(s) and table(s)
        public DbSet<Department> Departments { get; set; }
        //The above line is the association between the department model and the department table
        public DbSet<Product> Products { get; set; }        
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}