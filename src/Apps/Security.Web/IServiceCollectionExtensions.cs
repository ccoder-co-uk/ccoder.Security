// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Security.Web;

public static partial class IServiceCollectionExtensions
{
    public static string SSOUserId = "TestUser1";

    public static void AddAspNetCore(this IServiceCollection newIServiceCollection)
    {
        _ = newIServiceCollection.AddResponseCompression();

        _ = newIServiceCollection.AddMvcCore(setupAction: options =>
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

    public static void AddMetadata(this IServiceCollection newIServiceCollection)
    {
        _ = newIServiceCollection.AddEndpointsApiExplorer();
        _ = newIServiceCollection.AddSwaggerGen();
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