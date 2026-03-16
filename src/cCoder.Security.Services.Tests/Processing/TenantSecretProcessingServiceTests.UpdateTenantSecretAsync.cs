using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class TenantSecretProcessingServiceTests
{
    [Fact]
    public async Task ShouldUpdateTenantSecretAsync()
    {
        TenantSecret inputTenantSecret = RandomTenantSecret();
        string plainTextValue = inputTenantSecret.Value;
        string encryptedValue = Guid.NewGuid().ToString();

        cryptoMock.Setup(crypto =>
            crypto.Encrypt(plainTextValue))
            .Returns(encryptedValue);

        tenantSecretServiceMock.Setup(service =>
            service.UpdateTenantSecretAsync(It.Is<TenantSecret>(secret =>
                secret.Value == encryptedValue)))
            .ReturnsAsync((TenantSecret secret) => secret);

        TenantSecret actualTenantSecret =
            await tenantSecretProcessingService.UpdateTenantSecretAsync(inputTenantSecret);

        actualTenantSecret.Value.Should().Be(encryptedValue);

        cryptoMock.Verify(crypto => crypto.Encrypt(plainTextValue), Times.Once());
        tenantSecretServiceMock.Verify(service =>
            service.UpdateTenantSecretAsync(It.IsAny<TenantSecret>()), Times.Once());
        tenantSecretServiceMock.VerifyNoOtherCalls();
        cryptoMock.VerifyNoOtherCalls();
    }
}
