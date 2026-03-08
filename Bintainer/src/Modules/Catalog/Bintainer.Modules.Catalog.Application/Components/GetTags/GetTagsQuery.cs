using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Components.GetTags;

public sealed record GetTagsQuery() : IQuery<IReadOnlyCollection<string>>;
