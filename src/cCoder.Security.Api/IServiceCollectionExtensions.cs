using cCoder.Security.Api.EDM;
using cCoder.Security.Api.Interfaces;
using cCoder.Security.Objects;
using cCoder.Security.Services;
using Microsoft.AspNetCore.OData;

namespace cCoder.Security.Api;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityApi(
        this IServiceCollection services, 
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        SecurityConfiguration config = services.AddSecurityServices(configAction);

        services.AddApi();
        services.AddAspNet();

        if (!string.IsNullOrEmpty(config.RootPath))
            services.AddSecurityApiLayer(config.RootPath);
    }

    private static void AddApi(this IServiceCollection services) 
        => services.AddTransient<IAccountManager, AccountManager>();

    private static void AddAspNet(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient(ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext);
        services.AddTransient(ctx => ctx.GetService<HttpContext>()?.Request);
        services.AddTransient(ctx => ctx.GetService<HttpContext>()?.Session);
        services.AddSession();
    }

    public static void AddSecurityApiLayer(this IServiceCollection services, string atPath) 
        => services.AddControllers()
            .AddOData(opt =>
            {
                opt.Expand().Count().Filter().Select().OrderBy().SetMaxTop(1000);
                opt.AddRouteComponents(atPath, new SecurityModelBuilder().Build().EDMModel);
            });
}