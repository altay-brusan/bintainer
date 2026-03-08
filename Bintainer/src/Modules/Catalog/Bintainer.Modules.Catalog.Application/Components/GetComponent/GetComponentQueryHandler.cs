using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Domain.Components;

namespace Bintainer.Modules.Catalog.Application.Components.GetComponent;

internal sealed class GetComponentQueryHandler(
    IComponentReadService componentReadService) : IQueryHandler<GetComponentQuery, ComponentResponse>
{
    public async Task<Result<ComponentResponse>> Handle(GetComponentQuery request, CancellationToken cancellationToken)
    {
        var component = await componentReadService.GetByIdAsync(request.ComponentId, cancellationToken);

        if (component is null)
        {
            return Result.Failure<ComponentResponse>(ComponentErrors.NotFound(request.ComponentId));
        }

        return component;
    }
}
