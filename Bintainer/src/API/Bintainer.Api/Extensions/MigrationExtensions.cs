using Bintainer.Modules.ActivityLog.Infrastructure.Database;
using Bintainer.Modules.Catalog.Infrastructure.Database;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Bintainer.Modules.Users.Domain.Users;
using Bintainer.Modules.Users.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Api.Extensions;

internal static class MigrationExtensions
{
    internal static void ApplyMigrations(this IApplicationBuilder app)
    {
        using (IServiceScope scope = app.ApplicationServices.CreateScope())
        {
            ApplyMigration<UsersDbContext>(scope);
            ApplyMigration<InventoryDbContext>(scope);
            ApplyMigration<CatalogDbContext>(scope);
            ApplyMigration<ActivityLogDbContext>(scope);
        }

        SeedDefaultAdminAsync(app.ApplicationServices).GetAwaiter().GetResult();
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        context.Database.Migrate();
    }

    private static async Task SeedDefaultAdminAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

        const string email = "demo@bintainer.com";
        const string password = "Admin@123";

        if (await dbContext.DomainUsers.AnyAsync(u => u.Email == email))
        {
            return;
        }

        // Check if IdentityUser already exists (e.g. from a previous partial seed)
        IdentityUser? identityUser = await userManager.FindByEmailAsync(email);

        if (identityUser is null)
        {
            identityUser = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            IdentityResult result = await userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                logger.LogError("Failed to seed admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }
        }

        var user = User.Create(email, "Admin", "User", identityUser.Id);

        dbContext.DomainUsers.Add(user);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded default admin user: {Email}", email);
    }
}
