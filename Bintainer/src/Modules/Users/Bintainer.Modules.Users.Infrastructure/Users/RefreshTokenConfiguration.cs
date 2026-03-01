using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Users.Infrastructure.Users;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<Domain.Users.RefreshToken>
{
    public void Configure(EntityTypeBuilder<Domain.Users.RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token).HasMaxLength(500);
        builder.HasIndex(r => r.Token).IsUnique();

        builder.Property(r => r.UserId);
        builder.Property(r => r.ExpiresOnUtc);
        builder.Property(r => r.IsRevoked);
    }
}
