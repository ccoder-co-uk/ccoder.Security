// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;

using cCoder.Eventing;
using cCoder.Security;

namespace Security.HostedServices;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        builder.Services.AddHostedServices(
            configuration: builder.Configuration);

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();

        app.Services
            .GetRequiredService<IEventHub>()
            .ListenToSecurityEvents();

        app.UseSecurityHostedServicesApplication();
        app.Run();
    }
}