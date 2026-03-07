using Bintainer.Modules.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Users.Infrastructure.Database;

internal sealed class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId);
        builder.Property(p => p.Currency).HasMaxLength(10).HasDefaultValue("USD");

        builder.HasIndex(p => p.UserId).IsUnique();
    }
}
