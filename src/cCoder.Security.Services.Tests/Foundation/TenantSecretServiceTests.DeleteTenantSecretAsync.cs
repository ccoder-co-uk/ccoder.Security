using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TenantSecretServiceTests
{
    [Fact]
    public async Task DeleteTenantSecretAsyncWorksAsExpected()
    {
        TenantSecret inputTenantSecret = RandomTenantSecret();

        await tenantSecretService.DeleteTenantSecretAsync(inputTenantSecret);

        tenantSecretBrokerMock.Verify(broker =>
            broker.DeleteTenantSecretAsync(inputTenantSecret), Times.Once());

        tenantSecretBrokerMock.VerifyNoOtherCalls();
    }
}
