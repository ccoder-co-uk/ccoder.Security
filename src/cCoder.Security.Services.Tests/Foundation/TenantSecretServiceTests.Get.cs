using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TenantSecretServiceTests
{
    [Fact]
    public void GetAllTenantSecretsWorksAsExpected()
    {
        IQueryable<TenantSecret> expectedTenantSecrets = RandomTenantSecrets()
            .AsQueryable();

        tenantSecretBrokerMock.Setup(broker =>
            broker.GetAllTenantSecrets())
            .Returns(expectedTenantSecrets);

        IQueryable<TenantSecret> actualTenantSecrets =
            tenantSecretService.GetAllTenantSecrets();

        actualTenantSecrets.Should().BeEquivalentTo(expectedTenantSecrets);

        tenantSecretBrokerMock.Verify(broker =>
            broker.GetAllTenantSecrets(), Times.Once());

        tenantSecretBrokerMock.VerifyNoOtherCalls();
    }
}
