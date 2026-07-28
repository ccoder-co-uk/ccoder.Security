// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using cCoder.Security.Exposures.EventHandlers;

namespace Security.HostedServices;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseSecurityHostedServicesApplication(
        this WebApplication app)
    {
        app.MapGet(
            pattern: "/",
            handler: (IHostEnvironment environment) =>
                Results.Text(
                    content: BuildHostedServicesReport(
                        environment: environment),
                    contentType: "text/plain"));

        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "Healthy"));

        using IServiceScope serviceScope = app.Services.CreateScope();
        IServiceProvider services = serviceScope.ServiceProvider;

        foreach (ISecurityEventHandlers handlers
            in services.GetServices<ISecurityEventHandlers>())
        {
            handlers.ListenToAllEvents();
        }

        return app;
    }

    private static string BuildHostedServicesReport(
        IHostEnvironment environment) =>
        string.Join(
            separator: Environment.NewLine,
            value:
            [
                "cCoder.Security Hosted Services",
                "Status: Healthy",
                $"Environment: {environment.EnvironmentName}",
                "Health: /Health",
                string.Empty,
                "Hosted background services:",
                "- TokenCleaner -> ITokenService.DeleteExpiredAsync every 1 minute"
            ]);
}