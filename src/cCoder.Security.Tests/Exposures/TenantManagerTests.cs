using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Events;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public class TenantManagerTests
{
    private readonly Mock<ITenantSetupEventService> tenantSetupEventServiceMock;
    private readonly ITenantManager tenantManager;

    public TenantManagerTests()
    {
        tenantSetupEventServiceMock = new Mock<ITenantSetupEventService>(MockBehavior.Strict);
        tenantManager = new TenantManager(tenantSetupEventServiceMock.Object);
    }

    [Fact]
    public async Task ShouldRaiseTenantSetupEvent()
    {
        SetupDetails setupDetails = CreateSetupDetails();

        tenantSetupEventServiceMock
            .Setup(service => service.RaiseSetupAsync(setupDetails))
            .Returns(ValueTask.CompletedTask);

        await tenantManager.SetupAsync(setupDetails);

        tenantSetupEventServiceMock.Verify(service => service.RaiseSetupAsync(setupDetails), Times.Once);
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

