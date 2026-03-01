using Bintainer.Common.Domain;

namespace Bintainer.Modules.Users.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<Result<string>> RegisterAsync(string email, string password);
    Task<Result<string>> LoginAsync(string email, string password);
}
