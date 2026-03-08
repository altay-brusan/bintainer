using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.Reports.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bintainer.Modules.Reports.Infrastructure;

public static class ReportsModule
{
    public static IServiceCollection AddReportsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        services.AddScoped<IReportReadService, ReportReadService>();

        return services;
    }
}
