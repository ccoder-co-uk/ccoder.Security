using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class SSORoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAllowBootstrappingFirstRoleWithoutPortalAdminCheck()
    {
        SSORole inputRole = new()
        {
            Name = "Administrators",
            TenantId = "tenant-1"
        };

        roleProcessingServiceMock
            .Setup(x => x.GetAllSSORoles())
            .Returns(Array.Empty<SSORole>().AsQueryable());
        roleProcessingServiceMock
            .Setup(x => x.AddSSORoleAsync(inputRole))
            .ReturnsAsync(inputRole);

        SSORole actualRole = await roleOrchestrationService.AddSSORoleAsync(inputRole);

        actualRole.Should().BeSameAs(inputRole);
        authorizationBrokerMock.Verify(
            x => x.UserIsPortalAdminWithPrivilege(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ShouldRequirePortalAdminCheckAfterBootstrapRoleExists()
    {
        SSORole inputRole = new()
        {
            Name = "Editors",
            TenantId = "tenant-1"
        };

        roleProcessingServiceMock
            .Setup(x => x.GetAllSSORoles())
            .Returns(new[] { new SSORole { Name = "Administrators", TenantId = "tenant-1" } }.AsQueryable());
        authorizationBrokerMock
            .Setup(x => x.UserIsPortalAdminWithPrivilege("tenant_admin"));
        roleProcessingServiceMock
            .Setup(x => x.AddSSORoleAsync(inputRole))
            .ReturnsAsync(inputRole);

        await roleOrchestrationService.AddSSORoleAsync(inputRole);

        authorizationBrokerMock.Verify(
            x => x.UserIsPortalAdminWithPrivilege("tenant_admin"),
            Times.Once);
    }
}
