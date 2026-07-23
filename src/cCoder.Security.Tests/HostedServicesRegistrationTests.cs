// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace cCoder.Security.Tests;

public class HostedServicesRegistrationTests
{
    [Fact]
    public void AddSecurityHostedServicesRegistersTokenCleaner()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSecurityHostedServices(configAction: (_, _) => { });

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(ITokenCleaner)
                && descriptor.ImplementationType == typeof(TokenCleaner));

        Assert.Contains(
collection: services,
filter: descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationFactory is not null);
    }
}