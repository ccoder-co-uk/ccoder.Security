// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.EventHandlers;

namespace cCoder.Security;

public static class WebApplicationExtensions
{
    public static WebApplication StartSecurityWeb(this WebApplication app, ILogger log = null) =>
        app.UseSecurityExposure(log: log);

    public static WebApplication StartSecurityHostedServices(this WebApplication app) =>
        app.ListenToSecurityEvents();

    public static WebApplication UseSecurityExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation(message: "Initialising Security");
        return app;
    }

    public static WebApplication ListenToSecurityEvents(this WebApplication app)
    {
        using IServiceScope serviceScope = app.Services.CreateScope();
        IServiceProvider services = serviceScope.ServiceProvider;

        foreach (ISecurityEventHandlers handlers in services.GetServices<ISecurityEventHandlers>())
            handlers.ListenToAllEvents();

        return app;
    }
}