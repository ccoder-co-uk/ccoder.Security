using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processings.Events;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public class TenantManagerTests
{
    private readonly Mock<ITenantSetupEventProcessingService> tenantSetupEventProcessingServiceMock;
    private readonly ITenantManager tenantManager;

    public TenantManagerTests()
    {
        tenantSetupEventProcessingServiceMock = new Mock<ITenantSetupEventProcessingService>(MockBehavior.Strict);
        tenantManager = new TenantManager(tenantSetupEventProcessingServiceMock.Object);
    }

    [Fact]
    public async Task ShouldProcessTenantSetup()
    {
        SetupDetails setupDetails = CreateSetupDetails();

        tenantSetupEventProcessingServiceMock
            .Setup(service => service.SetupAsync(setupDetails))
            .Returns(ValueTask.CompletedTask);

        await tenantManager.SetupAsync(setupDetails);

        tenantSetupEventProcessingServiceMock.Verify(service => service.SetupAsync(setupDetails), Times.Once);
    }

    private static SetupDetails CreateSetupDetails() => new()
    {
        Tenant = new Tenant
        {
            Id = "default",
            Name = "Default"
        },
        User = new SSOUser
        {
            Id = "admin",
            DisplayName = "Admin User",
            Email = "admin@example.com",
            PasswordHash = "TestPass01!"
        }
    };
}

