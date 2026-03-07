using Bintainer.Modules.Users.Domain.Users;
using Bintainer.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Users.Infrastructure.Users;

internal sealed class UserPreferenceRepository(UsersDbContext dbContext) : IUserPreferenceRepository
{
    public async Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public void Insert(UserPreference preference)
    {
        dbContext.UserPreferences.Add(preference);
    }
}
