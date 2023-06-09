using Dogs.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Dogs.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Dog> Dogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Dog>().HasData(new Dog
            {
                Id = 1,
                Name = "Neo",
                Color = "red & amber",
                TailLength = 22,
                Weight = 32

            },
            new Dog
            {
                Id = 2,
                Name = "Jessy",
                Color = "black & white",
                TailLength = 7,
                Weight = 14

            });
        }
    }
}
