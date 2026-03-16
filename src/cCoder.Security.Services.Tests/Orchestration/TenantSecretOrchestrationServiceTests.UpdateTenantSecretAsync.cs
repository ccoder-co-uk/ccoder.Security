using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class TenantSecretOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldUpdateTenantSecretAsync()
    {
        TenantSecret existingTenantSecret = RandomTenantSecret();
        TenantSecret inputTenantSecret = RandomTenantSecret();

        inputTenantSecret.TenantId = existingTenantSecret.TenantId;
        inputTenantSecret.Key = existingTenantSecret.Key;

        tenantSecretProcessingServiceMock.Setup(service =>
            service.GetAllTenantSecrets())
            .Returns(new[] { existingTenantSecret }.AsQueryable());

        tenantSecretProcessingServiceMock.Setup(service =>
            service.UpdateTenantSecretAsync(existingTenantSecret))
            .ReturnsAsync(existingTenantSecret);

        TenantSecret actualTenantSecret =
            await tenantSecretOrchestrationService.UpdateTenantSecretAsync(
                existingTenantSecret.Id,
                inputTenantSecret);

        actualTenantSecret.Value.Should().BeNull();
        existingTenantSecret.Value.Should().Be(inputTenantSecret.Value);

        authBrokerMock.Verify(broker =>
            broker.UserHasPrivilege("tenantsecret_update", existingTenantSecret.TenantId), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service =>
            service.GetAllTenantSecrets(), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service =>
            service.UpdateTenantSecretAsync(existingTenantSecret), Times.Once());
        authBrokerMock.VerifyNoOtherCalls();
        tenantSecretProcessingServiceMock.VerifyNoOtherCalls();
    }
}
