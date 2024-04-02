using Microsoft.AspNetCore.OData;
using Security.Api.EDM;
using Security.Api.Interfaces;
using Security.Objects;
using Security.Services;

namespace Security.Api;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityApi(
        this IServiceCollection services, 
        Action<IServiceCollection, SecurityConfiguration> configAction)
    {
        var config = services.AddSecurityServices(configAction);

        services.AddApi();
        services.AddAspNet();

        if (!string.IsNullOrEmpty(config.RootPath))
            services.AddSecurityApiLayer(config.RootPath);
    }

    static void AddApi(this IServiceCollection services)
    {
        services.AddTransient<IAccountManager, AccountManager>();
    }

    static void AddAspNet(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient(ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext);
        services.AddTransient(ctx => ctx.GetService<HttpContext>()?.Request);
        services.AddTransient(ctx => ctx.GetService<HttpContext>()?.Session);
        services.AddSession();
    }

    public static void AddSecurityApiLayer(this IServiceCollection services, string atPath)
    {
        services.AddControllers().AddOData(opt =>
        {
            opt.Expand().Count().Filter().Select().OrderBy().SetMaxTop(1000);
            opt.AddRouteComponents(atPath, new SecurityModelBuilder().Build().EDMModel);
        });
    }
}