using Bintainer.Modules.ActivityLog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.ActivityLog.Infrastructure.Database;

public sealed class ActivityLogDbContext : DbContext
{
    public const string Schema = "activity";

    public ActivityLogDbContext(DbContextOptions<ActivityLogDbContext> options) : base(options) { }

    public DbSet<ActivityEntry> Activities => Set<ActivityEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActivityLogDbContext).Assembly);
    }
}
