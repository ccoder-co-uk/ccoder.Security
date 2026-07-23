// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OData;
using System.Security;
using Security.Web.Exposures;

namespace Security.Web;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSecurityWebApplication(
        this WebApplication app)
    {
        app.InitialiseSecurityDatabase();
        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "Healthy"));
        app.UseSession();
        app.StartSecurityWeb();
        app.UseTheFramework();

        return app;
    }

    public static IApplicationBuilder InitialiseSecurityDatabase(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using var db = scope.ServiceProvider
            .GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(ignoreAuthInfo: true);
        db.Migrate();

        return app;
    }

    public static IApplicationBuilder UseTheFramework(this IApplicationBuilder app)
    {
        app.HandleExceptions();

        app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseODataBatching();
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors(configurePolicy: builder =>
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.SetIsOriginAllowed(isOriginAllowed: _ => true);
            builder.AllowCredentials();
        });

        app.UseSwaggerUI()
            .UseSwagger()
            .UseODataRouteDebug();

        app.UseEndpoints(configure: endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Get}/{id?}");

            endpoints.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{action}");
        });

        return app;
    }

    private static IApplicationBuilder HandleExceptions(this IApplicationBuilder app)
        =>
        app.UseExceptionHandler(configure: errorApp => errorApp.Run(handler: async (context) =>
        {
            Exception ex = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
            context.Response.StatusCode = ex?.GetType() == typeof(SecurityException) ? 401 : 500;
            context.Response.ContentType = "application/json";

            if (ex != null)
            {
                await context.Response.WriteAsync("{ \"error\": \"" + ex.Message.Replace("\"", "\'") + "\" }");
                Exception innerEx = ex.InnerException;

                while (innerEx != null)
                { innerEx = innerEx.InnerException; }
            }
        }));
}