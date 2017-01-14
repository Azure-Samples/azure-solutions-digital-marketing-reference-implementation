using System.Data.Entity;

namespace AzureKit.Data.Sql.Models
{
    public class AzureKitDbContext : DbContext
    {
        public AzureKitDbContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

    }
}
