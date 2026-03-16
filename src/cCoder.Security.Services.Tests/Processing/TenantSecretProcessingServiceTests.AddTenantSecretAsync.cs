using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class TenantSecretProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddTenantSecretAsync()
    {
        TenantSecret inputTenantSecret = RandomTenantSecret();
        TenantSecret expectedTenantSecret = inputTenantSecret.DeepClone();
        string plainTextValue = inputTenantSecret.Value;
        string encryptedValue = Guid.NewGuid().ToString();

        cryptoMock.Setup(crypto =>
            crypto.Encrypt(plainTextValue))
            .Returns(encryptedValue);

        tenantSecretServiceMock.Setup(service =>
            service.AddTenantSecretAsync(It.Is<TenantSecret>(secret =>
                secret.Id == inputTenantSecret.Id &&
                secret.TenantId == inputTenantSecret.TenantId &&
                secret.Key == inputTenantSecret.Key &&
                secret.Value == encryptedValue)))
            .ReturnsAsync((TenantSecret secret) => secret);

        TenantSecret actualTenantSecret =
            await tenantSecretProcessingService.AddTenantSecretAsync(inputTenantSecret);

        expectedTenantSecret.Value = encryptedValue;

        actualTenantSecret.Should().BeEquivalentTo(expectedTenantSecret);

        cryptoMock.Verify(crypto => crypto.Encrypt(plainTextValue), Times.Once());
        tenantSecretServiceMock.Verify(service =>
            service.AddTenantSecretAsync(It.IsAny<TenantSecret>()), Times.Once());
        tenantSecretServiceMock.VerifyNoOtherCalls();
        cryptoMock.VerifyNoOtherCalls();
    }
}
