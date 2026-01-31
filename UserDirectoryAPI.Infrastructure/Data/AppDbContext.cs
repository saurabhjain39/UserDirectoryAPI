using Microsoft.EntityFrameworkCore;
using UserDirectoryAPI.Domain.Entities;

namespace UserDirectoryAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}
