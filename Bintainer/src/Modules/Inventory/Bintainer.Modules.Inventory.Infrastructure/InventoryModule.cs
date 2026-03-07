using Bintainer.Common.Infrastructure.Interceptors;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Abstractions.Storage;
using Bintainer.Modules.Inventory.Domain.BomImports;
using Bintainer.Modules.Inventory.Domain.Categories;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Domain.Footprints;
using Bintainer.Modules.Inventory.Domain.Inventories;
using Bintainer.Modules.Inventory.Domain.Movements;
using Bintainer.Modules.Inventory.Domain.StorageUnits;
using Bintainer.Modules.Inventory.Infrastructure.BomImports;
using Bintainer.Modules.Inventory.Infrastructure.Categories;
using Bintainer.Modules.Inventory.Infrastructure.Compartments;
using Bintainer.Modules.Inventory.Infrastructure.Components;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Bintainer.Modules.Inventory.Infrastructure.Footprints;
using Bintainer.Modules.Inventory.Infrastructure.Inventories;
using Bintainer.Modules.Inventory.Infrastructure.Movements;
using Bintainer.Modules.Inventory.Infrastructure.Storage;
using Bintainer.Modules.Inventory.Infrastructure.StorageUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bintainer.Modules.Inventory.Infrastructure;

public static class InventoryModule
{
    public static IServiceCollection AddInventoryModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddDbContext<InventoryDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InventoryDbContext>());

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IStorageUnitRepository, StorageUnitRepository>();
        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFootprintRepository, FootprintRepository>();
        services.AddScoped<ICompartmentRepository, CompartmentRepository>();
        services.AddScoped<IMovementRepository, MovementRepository>();
        services.AddScoped<IBomImportRepository, BomImportRepository>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
