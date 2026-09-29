// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Security.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

        builder.Services.AddWeb(
            configuration: builder.Configuration);

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();
        app.UseSecurityWebApplication();
        app.Run();
    }
}
