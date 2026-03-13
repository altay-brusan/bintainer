using Bintainer.Modules.ActivityLog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.ActivityLog.Infrastructure.Database;

public sealed class ActivityLogDbContext : DbContext
{
    public ActivityLogDbContext(DbContextOptions<ActivityLogDbContext> options) : base(options) { }

    public DbSet<ActivityEntry> Activities => Set<ActivityEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Activity);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActivityLogDbContext).Assembly);
    }
}
