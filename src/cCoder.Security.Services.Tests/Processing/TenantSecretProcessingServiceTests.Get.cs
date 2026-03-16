using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class TenantSecretProcessingServiceTests
{
    [Fact]
    public void ShouldGetTenantSecretsWithoutValues()
    {
        IQueryable<TenantSecret> tenantSecrets = RandomTenantSecrets()
            .AsQueryable();

        tenantSecretServiceMock.Setup(service =>
            service.GetAllTenantSecrets())
            .Returns(tenantSecrets);

        IQueryable<TenantSecret> actualTenantSecrets = tenantSecretProcessingService.GetAllTenantSecrets();

        actualTenantSecrets.Should().OnlyContain(secret => secret.Value == null);

        tenantSecretServiceMock.Verify(service => service.GetAllTenantSecrets(), Times.Once());
        tenantSecretServiceMock.VerifyNoOtherCalls();
        cryptoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldGetTenantSecretByTenantAndKeyDecryptedAsync()
    {
        TenantSecret tenantSecret = RandomTenantSecret();
        string decryptedValue = Guid.NewGuid().ToString();

        tenantSecretServiceMock.Setup(service =>
            service.GetAllTenantSecrets())
            .Returns(new[] { tenantSecret }.AsQueryable());

        cryptoMock.Setup(crypto =>
            crypto.Decrypt(tenantSecret.Value))
            .Returns(decryptedValue);

        string actualValue = tenantSecretProcessingService.GetDecryptedTenantSecretByKeyAsync(tenantSecret.TenantId, tenantSecret.Key);

        actualValue.Should().Be(decryptedValue);

        cryptoMock.Verify(crypto => crypto.Decrypt(tenantSecret.Value), Times.Once());
        tenantSecretServiceMock.Verify(service => service.GetAllTenantSecrets(), Times.Once());
        tenantSecretServiceMock.VerifyNoOtherCalls();
        cryptoMock.VerifyNoOtherCalls();
    }
}
