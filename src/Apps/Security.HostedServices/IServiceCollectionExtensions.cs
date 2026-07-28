// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security;
using Security.HostedServices.Models;

namespace Security.HostedServices;

public static class IServiceCollectionExtensions
{
    public static void AddSecurityHostedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SecurityHostedServicesConfiguration> configure = null)
    {
        SecurityHostedServicesConfiguration applicationConfiguration = new();
        configuration.Bind(applicationConfiguration);
        configure?.Invoke(applicationConfiguration);

        services.AddSecurityHostedServices(
            applicationConfiguration.Security);
    }
}