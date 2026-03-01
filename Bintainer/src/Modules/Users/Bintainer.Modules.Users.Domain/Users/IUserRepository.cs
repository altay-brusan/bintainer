namespace Bintainer.Modules.Users.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(User user);
}
