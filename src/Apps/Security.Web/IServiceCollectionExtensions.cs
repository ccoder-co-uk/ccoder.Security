// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using cCoder.Security.Exposures;
using Security.Web.Exposures;
using Security.Web.Models;

namespace Security.Web;

public static partial class IServiceCollectionExtensions
{
    public static void AddSecurityWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SecurityWebConfiguration> configure = null)
    {
        SecurityWebConfiguration applicationConfiguration = new();
        configuration.Bind(applicationConfiguration);
        configure?.Invoke(applicationConfiguration);

        services.AddAspNetCore();
        services.AddMetadata();
        services.AddSecurityWeb(applicationConfiguration.Security);
        services.AddControllersWithViews();
        services.AddSessions();
        services.AddTransient<IHomeManager, HomeManager>();
        services.AddTransient<ICurrentUserManager, CurrentUserManager>();
    }

    private static void AddAspNetCore(this IServiceCollection services)
    {
        _ = services.AddResponseCompression();

        IMvcCoreBuilder mvcCoreBuilder =
            services.AddMvcCore(setupAction: options =>
        {
            options.MaxIAsyncEnumerableBufferLimit = int.MaxValue;
            options.MaxModelBindingCollectionSize = 10000;
            options.MaxModelBindingRecursionDepth = 10;
        });

        mvcCoreBuilder.AddDataAnnotations();
        mvcCoreBuilder.AddCors(
            setupAction: options =>
                options.AddDefaultPolicy(configurePolicy: builder =>
            {
                _ = builder.AllowAnyHeader();
                _ = builder.AllowAnyMethod();
                _ = builder.SetIsOriginAllowed(origin => true);
                _ = builder.AllowCredentials();
            }));
    }

    private static void AddMetadata(this IServiceCollection services)
    {
        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();
    }

    private static void AddSessions(this IServiceCollection services)
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