using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class TenantProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteTenantAsync()
    {
        //given
        Tenant inputTenant = RandomTenant();

        //when
        await tenantProcessingService.DeleteTenantAsync(inputTenant);

        //then
        tenantServiceMock.Verify(tenantServiceMock =>
            tenantServiceMock.DeleteTenantAsync(inputTenant),
            Times.Once());

        tenantServiceMock.VerifyNoOtherCalls();
    }
}
