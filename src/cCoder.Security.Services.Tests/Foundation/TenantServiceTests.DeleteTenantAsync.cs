using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TenantServiceTests
{
    [Fact]
    public async void DeleteTenantAsyncWorksAsExpected()
    {
        //given
        Tenant inputTenant = RandomTenant();

        //when
        await tenantService.DeleteTenantAsync(inputTenant);

        //then
        tenantBrokerMock.Verify(tenantBrokerMock =>
            tenantBrokerMock.DeleteTenantAsync(inputTenant),
            Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}
