using cCoder.Security.Api;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.MSSQL;

namespace SecurityMSSQL;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "ENV_")
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.testing.json", optional: true, reloadOnChange: true)
            .Build();

        // configure DI for stack
        builder.Services.AddAspNetCore();
        builder.Services.AddMetadata();

        builder.Services.AddSecurityApi((services, securityConfig) =>
        {
            securityConfig.RootPath = "Api/Security";
            securityConfig.AddEntityFramework(services);

            securityConfig.AddMSSQLModelProvider(
                services,
                config.GetConnectionString("SSO"));

            securityConfig.UseAESHMMACPasswordEncryption(
                services,
                config.GetSection("settings")["DecryptionKey"]);
        });

        builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

        builder.Services.ConfigureSessions();

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();
        app.UseSession();
        app.UseTheFramework();
        app.Run();
    }
}