namespace Bintainer.Modules.Catalog.Application.Abstractions.Storage;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
}
