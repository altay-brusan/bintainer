using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.Users.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler(
    IUserRepository userRepository) : IQueryHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    public async Task<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<CurrentUserResponse>(UserErrors.NotFound(request.UserId));
        }

        return new CurrentUserResponse(user.Id, user.Email, user.FirstName, user.LastName);
    }
}
