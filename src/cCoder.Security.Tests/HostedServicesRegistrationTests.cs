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

        services.AddSecurityHostedServices((_, _) => { });

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(ITokenCleaner)
                && descriptor.ImplementationType == typeof(TokenCleaner));

        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IHostedService)
                && descriptor.ImplementationFactory is not null);
    }
}
