using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Caching;
using Bintainer.Common.Application.Clock;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.EventBus;
using Bintainer.Common.Infrastructure.Authorization;
using Bintainer.Common.Infrastructure.Caching;
using Bintainer.Common.Infrastructure.Clock;
using Bintainer.Common.Infrastructure.Data;
using Bintainer.Common.Infrastructure.Interceptors;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace Bintainer.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        params Action<IRegistrationConfigurator>[] moduleConfigureConsumers)
    {
        var npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString)
            .EnableDynamicJson()
            .Build();
        services.AddSingleton(npgsqlDataSource);

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddDistributedMemoryCache();
        services.AddScoped<ICacheService, CacheService>();

        services.AddMassTransit(config =>
        {
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(config);
            }

            config.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        services.TryAddScoped<IEventBus, EventBus.EventBus>();

        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
