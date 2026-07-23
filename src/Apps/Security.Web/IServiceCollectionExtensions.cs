// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Exposures;
using Security.Web.Exposures;

namespace Security.Web;

public static partial class IServiceCollectionExtensions
{
    public static string SSOUserId = "TestUser1";

    public static void AddAspNetCore(this IServiceCollection services)
    {
        _ = services.AddResponseCompression();

        _ = services.AddMvcCore(setupAction: options =>
        {
            options.MaxIAsyncEnumerableBufferLimit = int.MaxValue;
            options.MaxModelBindingCollectionSize = 10000;
            options.MaxModelBindingRecursionDepth = 10;
        })
            .AddDataAnnotations()
            .AddCors(setupAction: options => options.AddDefaultPolicy(configurePolicy: builder =>
            {
                _ = builder.AllowAnyHeader();
                _ = builder.AllowAnyMethod();
                _ = builder.SetIsOriginAllowed(origin => true);
                _ = builder.AllowCredentials();
            }));
    }

    public static void AddSecurityWebApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAspNetCore();
        services.AddMetadata();

        services.AddSecurityWeb(configAction: (serviceCollection, securityConfig) =>
        {
            securityConfig.RootPath = "Api/Security";

            securityConfig.AddMSSQLModelProvider(
                services: serviceCollection,
                connectionString: configuration.GetConnectionString("SSO"));

            securityConfig.UseAESHMMACPasswordEncryption(
                services: serviceCollection,
                decryptionKey: configuration.GetSection("settings")["DecryptionKey"]);
        });

        services.AddControllersWithViews();
        services.ConfigureSessions();
        services.AddTransient<IUIBaselineManager, UIBaselineManager>();
        services.AddTransient<IHomeManager, HomeManager>();
        services.AddTransient<ICurrentUserManager, CurrentUserManager>();
    }

    public static void AddMetadata(this IServiceCollection services)
    {
        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();
    }

    public static void ConfigureSessions(this IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(configureOptions: options =>
        {
            options.Secure = CookieSecurePolicy.Always;
            options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
        });

        services.AddSession(configure: options =>
        {
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.HttpOnly = true;
        });
    }
}