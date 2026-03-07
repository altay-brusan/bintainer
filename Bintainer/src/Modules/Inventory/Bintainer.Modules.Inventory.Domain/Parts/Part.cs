using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Parts;

public sealed class Part : Entity
{
    private Part() { }

    public string PartNumber { get; private set; } = string.Empty;
    public string ManufacturerPartNumber { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? DetailedDescription { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? Url { get; private set; }
    public string? Provider { get; private set; }
    public string? ProviderPartNumber { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? FootprintId { get; private set; }
    public Dictionary<string, string> Attributes { get; private set; } = [];
    public string? Tags { get; private set; }

    public static Part Create(
        string partNumber,
        string manufacturerPartNumber,
        string description,
        string? detailedDescription,
        string? imageUrl,
        string? url,
        string? provider,
        string? providerPartNumber,
        Guid? categoryId,
        Guid? footprintId,
        Dictionary<string, string>? attributes,
        string? tags)
    {
        var part = new Part
        {
            Id = Guid.NewGuid(),
            PartNumber = partNumber,
            ManufacturerPartNumber = manufacturerPartNumber,
            Description = description,
            DetailedDescription = detailedDescription,
            ImageUrl = imageUrl,
            Url = url,
            Provider = provider,
            ProviderPartNumber = providerPartNumber,
            CategoryId = categoryId,
            FootprintId = footprintId,
            Attributes = attributes ?? [],
            Tags = tags
        };

        part.Raise(new PartCreatedDomainEvent(part.Id));

        return part;
    }

    public void Update(
        string partNumber,
        string manufacturerPartNumber,
        string description,
        string? detailedDescription,
        string? imageUrl,
        string? url,
        string? provider,
        string? providerPartNumber,
        Guid? categoryId,
        Guid? footprintId,
        Dictionary<string, string>? attributes,
        string? tags)
    {
        PartNumber = partNumber;
        ManufacturerPartNumber = manufacturerPartNumber;
        Description = description;
        DetailedDescription = detailedDescription;
        ImageUrl = imageUrl;
        Url = url;
        Provider = provider;
        ProviderPartNumber = providerPartNumber;
        CategoryId = categoryId;
        FootprintId = footprintId;
        Attributes = attributes ?? [];
        Tags = tags;
    }
}
