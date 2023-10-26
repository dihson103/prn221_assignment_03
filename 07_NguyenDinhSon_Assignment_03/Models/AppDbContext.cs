using Microsoft.EntityFrameworkCore;

namespace _07_NguyenDinhSon_Assignment_03.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<AppUsers> AppUsers { get; set; }
        public virtual DbSet<PostCategories> PostCategories { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }

    }
}
