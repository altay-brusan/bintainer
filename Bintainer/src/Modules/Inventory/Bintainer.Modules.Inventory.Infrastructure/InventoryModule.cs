using Bintainer.Common.Infrastructure.Interceptors;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Inventories;
using Bintainer.Modules.Inventory.Application.Movements;
using Bintainer.Modules.Inventory.Application.Reports;
using Bintainer.Modules.Inventory.Application.StorageUnits;
using Bintainer.Modules.Inventory.Domain.Bins;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Inventories;
using Bintainer.Modules.Inventory.Domain.Movements;
using Bintainer.Modules.Inventory.Domain.StorageUnits;
using Bintainer.Modules.Inventory.Infrastructure.Bins;
using Bintainer.Modules.Inventory.Infrastructure.Compartments;
using Bintainer.Modules.Inventory.Infrastructure.Consumers;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Bintainer.Modules.Inventory.Infrastructure.Inventories;
using Bintainer.Modules.Inventory.Infrastructure.Movements;
using Bintainer.Modules.Inventory.Infrastructure.Reports;
using Bintainer.Modules.Inventory.Infrastructure.StorageUnits;
using MassTransit;
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
        services.AddScoped<IBinRepository, BinRepository>();
        services.AddScoped<ICompartmentRepository, CompartmentRepository>();
        services.AddScoped<IMovementRepository, MovementRepository>();

        services.AddScoped<IReportReadService, ReportReadService>();
        services.AddScoped<IInventoryReadService, InventoryReadService>();
        services.AddScoped<IStorageUnitReadService, StorageUnitReadService>();
        services.AddScoped<IMovementReadService, MovementReadService>();

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<ComponentDeletedIntegrationEventConsumer>();
    }
}
