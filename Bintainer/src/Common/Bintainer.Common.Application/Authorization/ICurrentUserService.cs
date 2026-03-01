namespace Bintainer.Common.Application.Authorization;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
