namespace Bintainer.Modules.Users.Domain.Users;

public interface IUserPreferenceRepository
{
    Task<UserPreference?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    void Insert(UserPreference preference);
}
