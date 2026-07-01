using cCoder.Security;
using cCoder.Security.Data.EF;

namespace Security.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "ENV_")
            .Build();

        builder.Services.AddSecurityHostedServices((services, securityConfig) =>
        {
            securityConfig.AddMSSQLModelProvider(
                services,
                config.GetConnectionString("SSO"));

            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                config.GetSection("settings")["DecryptionKey"]);
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();
        app.MapGet("/", (IHostEnvironment environment) =>
            Results.Text(BuildHostedServicesReport(environment), "text/plain"));
        app.MapGet("/Health", () => Results.Text("Healthy"));
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
