using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.ActivityLog.Application.GetActivityLog;
using Bintainer.Modules.ActivityLog.Infrastructure.Consumers;
using Bintainer.Modules.ActivityLog.Infrastructure.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bintainer.Modules.ActivityLog.Infrastructure;

public static class ActivityLogModule
{
    public static IServiceCollection AddActivityLogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddDbContext<ActivityLogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddScoped<IActivityLogReadService, ActivityLogReadService>();

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<ActivityLoggedIntegrationEventConsumer>();
    }
}
