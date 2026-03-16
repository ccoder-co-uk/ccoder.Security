using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TenantSecretServiceTests
{
    [Fact]
    public async Task AddTenantSecretAsyncWorksAsExpected()
    {
        TenantSecret inputTenantSecret = RandomTenantSecret();
        TenantSecret expectedTenantSecret = inputTenantSecret.DeepClone();

        tenantSecretBrokerMock.Setup(broker =>
            broker.AddTenantSecretAsync(inputTenantSecret))
            .ReturnsAsync(inputTenantSecret);

        TenantSecret actualTenantSecret =
            await tenantSecretService.AddTenantSecretAsync(inputTenantSecret);

        actualTenantSecret.CreatedOn.Should()
            .BeCloseTo(actualTenantSecret.UpdatedOn, TimeSpan.FromSeconds(1));

        expectedTenantSecret.CreatedOn = actualTenantSecret.CreatedOn;
        expectedTenantSecret.UpdatedOn = actualTenantSecret.UpdatedOn;

        actualTenantSecret.Should().BeEquivalentTo(expectedTenantSecret);

        tenantSecretBrokerMock.Verify(broker =>
            broker.AddTenantSecretAsync(inputTenantSecret), Times.Once());

        tenantSecretBrokerMock.VerifyNoOtherCalls();
    }
}
