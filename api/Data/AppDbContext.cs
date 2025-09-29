using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<Test> Products { get; set; }
        // public DbSet<Category> Categories { get; set; }
    }
}
