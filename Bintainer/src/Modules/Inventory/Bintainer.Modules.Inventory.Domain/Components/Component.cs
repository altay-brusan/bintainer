using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Components;

public sealed class Component : Entity
{
    private Component() { }

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
    public decimal? UnitPrice { get; private set; }
    public string? Manufacturer { get; private set; }
    public int LowStockThreshold { get; private set; }

    public static Component Create(
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
        string? tags,
        decimal? unitPrice,
        string? manufacturer,
        int lowStockThreshold)
    {
        var component = new Component
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
            Tags = tags,
            UnitPrice = unitPrice,
            Manufacturer = manufacturer,
            LowStockThreshold = lowStockThreshold
        };

        component.Raise(new ComponentCreatedDomainEvent(component.Id));

        return component;
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
        string? tags,
        decimal? unitPrice,
        string? manufacturer,
        int lowStockThreshold)
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
        UnitPrice = unitPrice;
        Manufacturer = manufacturer;
        LowStockThreshold = lowStockThreshold;
    }

    public void UpdateImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
    }
}
