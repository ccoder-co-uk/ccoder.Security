// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;

namespace Security.HostedServices;

public static class IApplicationBuilderExtensions
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

        app.StartSecurityHostedServices();

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