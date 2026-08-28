// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using cCoder.Security.Data.EF;
using Security.HostedServices.Models;

namespace Security.HostedServices;

public static class IServiceCollectionExtensions
{
    public static void AddHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AppConfiguration> configure = null)
    {
        AppConfiguration applicationConfiguration = new();
        configuration.Bind(applicationConfiguration);
        configure?.Invoke(applicationConfiguration);

        services.AddSecurityData(applicationConfiguration.SecurityData);
        services.AddSecurityHostedServices(
            applicationConfiguration.Security);
    }
}