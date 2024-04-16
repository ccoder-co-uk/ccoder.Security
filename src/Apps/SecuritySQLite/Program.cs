using cCoder.Security.Api;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.SQLite;

namespace cCoder.SecuritySQLite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = new ConfigurationBuilder()
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

                securityConfig.AddSQLiteModelProvider(
                    services, 
                    config.GetConnectionString("SSO"));

                securityConfig.UseSHA512PasswordEncryption(services);
            });

            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromMinutes(60);
            });

            builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

            builder.Logging.ClearProviders();
            builder.Logging.AddSimpleConsole();

            var app = builder.Build();
            app.UseSession();
            app.UseTheFramework();
            app.Run();
        }
    }
}