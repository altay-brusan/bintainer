namespace Bintainer.Api.Extensions;

internal static class ConfigurationExtensions
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder builder, string[] modules)
    {
        foreach (string module in modules)
        {
            builder.AddJsonFile($"modules.{module}.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile($"modules.{module}.Development.json", optional: true, reloadOnChange: true);
        }
    }
}
