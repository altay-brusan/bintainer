using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Users.Infrastructure.Database;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : IdentityDbContext<IdentityUser>(options), IUnitOfWork
{
    public DbSet<User> DomainUsers => Set<User>();
    public DbSet<Domain.Users.RefreshToken> RefreshTokens => Set<Domain.Users.RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.Users);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}
