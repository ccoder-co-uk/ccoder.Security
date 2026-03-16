using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class TenantSecretOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldDeleteTenantSecretAsync()
    {
        TenantSecret existingTenantSecret = RandomTenantSecret();

        tenantSecretProcessingServiceMock.Setup(service =>
            service.GetAllTenantSecrets())
            .Returns(new[] { existingTenantSecret }.AsQueryable());

        await tenantSecretOrchestrationService.DeleteTenantSecretAsync(existingTenantSecret.Id);

        authBrokerMock.Verify(broker =>
            broker.UserHasPrivilege("tenantsecret_delete", existingTenantSecret.TenantId), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service =>
            service.GetAllTenantSecrets(), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service =>
            service.DeleteTenantSecretAsync(existingTenantSecret), Times.Once());
        authBrokerMock.VerifyNoOtherCalls();
        tenantSecretProcessingServiceMock.VerifyNoOtherCalls();
    }
}
