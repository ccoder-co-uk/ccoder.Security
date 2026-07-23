// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using cCoder.Security.Exposures;
using cCoder.Security.Data.EF;

namespace Security.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "ENV_")
            .Build();

        // configure DI for stack
        builder.Services.AddAspNetCore();
        builder.Services.AddMetadata();

        builder.Services.AddSecurityWeb(configAction: (services, securityConfig) =>
        {
            securityConfig.RootPath = "Api/Security";

            securityConfig.AddMSSQLModelProvider(
                services,
                config.GetConnectionString("SSO"));

            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                config.GetSection("settings")["DecryptionKey"]);
        });

        builder.Services.AddControllersWithViews();

        builder.Services.ConfigureSessions();

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();
        app.InitialiseSecurityDatabase();
        app.MapGet(pattern: "/Health", handler: () => Results.Text("Healthy"));
        app.UseSession();
        app.StartSecurityWeb();
        app.UseTheFramework();
        app.Run();
    }
}