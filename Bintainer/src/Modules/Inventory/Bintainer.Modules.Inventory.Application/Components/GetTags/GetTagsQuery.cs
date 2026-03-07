using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Components.GetTags;

public sealed record GetTagsQuery() : IQuery<IReadOnlyCollection<string>>;
