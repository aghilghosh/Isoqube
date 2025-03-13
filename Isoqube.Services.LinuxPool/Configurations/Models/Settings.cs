using Isoqube.Orchestration.Core.Configurations.Models;
using Isoqube.Orchestration.Core.ServiceBus;

namespace Isoqube.Services.LinuxPool.Configurations.Models
{
    public class AppSettings : AppSettingsBase
    {
        public static ServiceBusOptions? ServiceBusOptions { get; set; }
    }

    public class EnvSettings : EnvSettingsBase
    {
    }

    public class JobConfiguration : JobConfigurationBase
    {
    }
}
