using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class TenantSecretOrchestrationServiceTests
{
    [Fact]
    public void ShouldGetAllTenantSecretsForReadableTenants()
    {
        TenantSecret readableTenantSecret = RandomTenantSecret();
        TenantSecret hiddenTenantSecret = RandomTenantSecret();
        hiddenTenantSecret.TenantId = Guid.NewGuid().ToString();

        authBrokerMock.Setup(broker => broker.GetCurrentUser())
            .Returns(CreateUserWithTenantSecretReadRole(readableTenantSecret.TenantId));

        tenantSecretProcessingServiceMock.Setup(service =>
            service.GetAllTenantSecrets())
            .Returns(new[] { readableTenantSecret, hiddenTenantSecret }.AsQueryable());

        IQueryable<TenantSecret> actualTenantSecrets = tenantSecretOrchestrationService.GetAllTenantSecrets();

        actualTenantSecrets.Should().ContainSingle(secret => secret.TenantId == readableTenantSecret.TenantId);

        authBrokerMock.Verify(broker => broker.GetCurrentUser(), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service => service.GetAllTenantSecrets(), Times.Once());
        authBrokerMock.VerifyNoOtherCalls();
        tenantSecretProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldGetTenantSecretByKeyDecryptedAsync()
    {
        TenantSecret tenantSecret = RandomTenantSecret();

        tenantSecretProcessingServiceMock.Setup(service => service.GetDecryptedTenantSecretByKeyAsync(tenantSecret.TenantId, tenantSecret.Key))
            .Returns(tenantSecret.Value);

        string actualSecretValue = tenantSecretOrchestrationService.GetTenantSecretByKeyDecryptedAsync(tenantSecret.TenantId, tenantSecret.Key);

        actualSecretValue.Should().BeSameAs(tenantSecret.Value);

        authBrokerMock.Verify(broker => broker.UserHasPrivilege("tenantsecret_read", tenantSecret.TenantId), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service => service.GetDecryptedTenantSecretByKeyAsync(tenantSecret.TenantId, tenantSecret.Key), Times.Once());
        authBrokerMock.VerifyNoOtherCalls();
        tenantSecretProcessingServiceMock.VerifyNoOtherCalls();
    }
}