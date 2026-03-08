using Bintainer.Modules.Catalog.Application.Abstractions.Storage;

namespace Bintainer.Modules.Catalog.Infrastructure.Storage;

internal sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService()
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(_basePath, uniqueName);

        await using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return $"/uploads/{uniqueName}";
    }
}
