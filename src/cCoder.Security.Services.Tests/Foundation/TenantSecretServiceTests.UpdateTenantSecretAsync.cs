using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TenantSecretServiceTests
{
    [Fact]
    public async Task UpdateTenantSecretAsyncWorksAsExpected()
    {
        TenantSecret inputTenantSecret = RandomTenantSecret();
        TenantSecret expectedTenantSecret = inputTenantSecret.DeepClone();

        tenantSecretBrokerMock.Setup(broker =>
            broker.UpdateTenantSecretAsync(inputTenantSecret))
            .ReturnsAsync(inputTenantSecret);

        TenantSecret actualTenantSecret =
            await tenantSecretService.UpdateTenantSecretAsync(inputTenantSecret);

        actualTenantSecret.UpdatedOn.Should()
            .BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        expectedTenantSecret.UpdatedOn = actualTenantSecret.UpdatedOn;

        actualTenantSecret.Should().BeEquivalentTo(expectedTenantSecret);

        tenantSecretBrokerMock.Verify(broker =>
            broker.UpdateTenantSecretAsync(inputTenantSecret), Times.Once());

        tenantSecretBrokerMock.VerifyNoOtherCalls();
    }
}
