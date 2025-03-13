using Microsoft.Extensions.Configuration;


namespace Isoqube.Orchestration.Core.Extensions
{
    public static class ConfigurationBuilderExtension
    {
        public static void InitializeConfiguration(this IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables();
            builder.AddJsonFile("Configurations/appSettingsBase.json");
            builder.AddJsonFile("Configurations/appSettings.json");
            builder.AddJsonFile("Configurations/envSettings.json");
            builder.AddJsonFile("Configurations/jobConfig.json");
        }
    }
}

