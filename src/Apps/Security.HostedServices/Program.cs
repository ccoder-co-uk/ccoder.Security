// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using cCoder.Security.Data.EF;

namespace Security.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(basePath: Directory.GetCurrentDirectory())
            .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "ENV_")
            .Build();

        builder.Services.AddSecurityHostedServices(configAction: (services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(
newIServiceCollection: services,
connectionString: config.GetConnectionString("SSO"));

            securityConfig.UseAESHMMACPasswordEncryption(
services: services,
decryptionKey: config.GetSection("settings")["DecryptionKey"]);

            securityConfig.IsMigrating =
                config.GetValue<int?>(key: "MIGRATING") == 1
                || config.GetValue<bool?>(key: "Security:IsMigrating") == true;
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();

        app.MapGet(pattern: "/", handler: (IHostEnvironment environment) =>
            Results.Text(content: BuildHostedServicesReport(environment), contentType: "text/plain"));

        app.MapGet(pattern: "/Health", handler: () => Results.Text(content: "Healthy"));
        app.StartSecurityHostedServices();
        app.Run();
    }

    private static string BuildHostedServicesReport(IHostEnvironment environment) =>
        string.Join(
            Environment.NewLine,
            "cCoder.Security Hosted Services",
            $"Status: Healthy",
            $"Environment: {environment.EnvironmentName}",
            $"Health: /Health",
            string.Empty,
            "Hosted background services:",
            "- TokenCleaner -> ITokenService.DeleteExpiredAsync every 1 minute");
}