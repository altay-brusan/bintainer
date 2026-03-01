using System.Security.Claims;
using Bintainer.Common.Application.Authorization;
using Microsoft.AspNetCore.Http;

namespace Bintainer.Common.Infrastructure.Authorization;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            string? userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out Guid parsed) ? parsed : Guid.Empty;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
