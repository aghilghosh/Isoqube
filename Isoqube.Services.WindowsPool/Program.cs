using Isoqube.Endpoint;
using Isoqube.Services.WindowsPool.Configurations.Models;
using Isoqube.Orchestration.Core;

Environment.SetEnvironmentVariable("HOST", "http://localhost:5002");
Environment.SetEnvironmentVariable("APPLICATION_NAME", "Isoqube.Services.WindowsPool");
Environment.SetEnvironmentVariable("ENVIRONMENT", "DEVELOPMENT");
Environment.SetEnvironmentVariable("VERSION", "1.0.0.1");

var appBootstrapManager = new Bootstrapper<EnvSettings, AppSettings, JobConfiguration>();
await appBootstrapManager.BootstrapService<Startup>(args);