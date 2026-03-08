using Bintainer.Common.Infrastructure.Interceptors;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Application.Abstractions.Storage;
using Bintainer.Modules.Catalog.Application.BomImports;
using Bintainer.Modules.Catalog.Application.Categories;
using Bintainer.Modules.Catalog.Application.Components;
using Bintainer.Modules.Catalog.Application.Footprints;
using Bintainer.Modules.Catalog.Domain.BomImports;
using Bintainer.Modules.Catalog.Domain.Categories;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.Domain.Footprints;
using Bintainer.Modules.Catalog.Infrastructure.BomImports;
using Bintainer.Modules.Catalog.Infrastructure.Categories;
using Bintainer.Modules.Catalog.Infrastructure.Components;
using Bintainer.Modules.Catalog.Infrastructure.Database;
using Bintainer.Modules.Catalog.Infrastructure.Footprints;
using Bintainer.Modules.Catalog.Infrastructure.Storage;
using Bintainer.Modules.Catalog.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bintainer.Modules.Catalog.Infrastructure;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IFootprintRepository, FootprintRepository>();
        services.AddScoped<IBomImportRepository, BomImportRepository>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICatalogApi, CatalogApi>();

        services.AddScoped<IComponentReadService, ComponentReadService>();
        services.AddScoped<ICategoryReadService, CategoryReadService>();
        services.AddScoped<IFootprintReadService, FootprintReadService>();
        services.AddScoped<IBomImportReadService, BomImportReadService>();

        return services;
    }
}
