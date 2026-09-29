// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Hosting;
using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Security.HostedServices;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseSecurityHostedServicesApplication(
        this WebApplication app)
    {
        IHostEnvironment environment = app.Services
            .GetRequiredService<IHostEnvironment>();

        app.MapGet(
            pattern: "/",
            handler: () =>
                Results.Text(
                    content: BuildHostedServicesReport(
                        environment: environment),
                    contentType: "text/plain"));

        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "Healthy"));

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
