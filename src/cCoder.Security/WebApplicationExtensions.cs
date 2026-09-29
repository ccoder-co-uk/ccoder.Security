// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using cCoder.Security.Models.Configurations;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security;

public static class WebApplicationExtensions
{
    public static WebApplication StartSecurityWeb(this WebApplication app, ILogger log = null) =>
        app.UseSecurityExposure(log: log);

    public static WebApplication UseSecurityExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation(message: "Initialising Security");
        app.Use(middleware: ResolveAuthenticationAsync);
        return app;
    }

    private static async Task ResolveAuthenticationAsync(
        HttpContext context,
        RequestDelegate next)
    {
        SSOAuthInfo requestAuthInfo = context.RequestServices
            .GetRequiredService<SSOAuthInfo>();

        ISSOAuthInfo resolvedAuthInfo = await context.RequestServices
            .GetRequiredService<ISSOAuthInfoAggregationService>()
            .GetSSOAuthInfoAsync();

        requestAuthInfo.SSOUserId = resolvedAuthInfo.SSOUserId;
        requestAuthInfo.AuthenticationFailed =
            resolvedAuthInfo.AuthenticationFailed;

        if (!requestAuthInfo.AuthenticationFailed
            && !string.IsNullOrWhiteSpace(requestAuthInfo.SSOUserId)
            && !string.Equals(
                requestAuthInfo.SSOUserId,
                "Guest",
                StringComparison.OrdinalIgnoreCase))
        {
            ClaimsIdentity identity = new(
                claims:
                [
                    new Claim(
                        type: ClaimTypes.Name,
                        value: requestAuthInfo.SSOUserId)
                ],
                authenticationType: "cCoder.Security");

            context.User = new ClaimsPrincipal(identity);
        }

        await next(context: context);
    }
}