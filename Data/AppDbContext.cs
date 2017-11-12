using Microsoft.EntityFrameworkCore;

namespace HostedServiceSample.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Word> Words { get; set; }
    }
}
