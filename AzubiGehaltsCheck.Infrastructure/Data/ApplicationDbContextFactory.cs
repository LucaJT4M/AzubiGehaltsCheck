using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AzubiGehaltsCheck.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            // Only used for generating migrations
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=appdb;Username=postgres;Password=design-time-only");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}