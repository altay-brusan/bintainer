using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Components.UploadComponentImage;

public sealed record UploadComponentImageCommand(
    Guid ComponentId,
    Stream FileStream,
    string FileName,
    string ContentType) : ICommand<string>;
