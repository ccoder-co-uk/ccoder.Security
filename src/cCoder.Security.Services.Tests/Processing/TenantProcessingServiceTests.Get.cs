using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class TenantProcessingServiceTests
{
    [Fact]
    public void ShouldGetTenants()
    {
        //given
        IQueryable<Tenant> expectedTenants = RandomTenants()
            .AsQueryable();

        tenantServiceMock.Setup(tenantServiceMock =>
            tenantServiceMock.GetAllTenants())
            .Returns(expectedTenants);

        //when
        IQueryable<Tenant> actualTenants = tenantProcessingService.GetAllTenants();

        //then
        tenantServiceMock.Verify(tenantServiceMock =>
            tenantServiceMock.GetAllTenants(),
            Times.Once());

        tenantServiceMock.VerifyNoOtherCalls();
    }
}
