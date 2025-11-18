using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class TenantProcessingServiceTests
{
    [Fact]
    public async Task ShouldUpdateTenantAsync()
    {
        //given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();

        tenantServiceMock.Setup(tenantServiceMock =>
            tenantServiceMock.UpdateTenantAsync(inputTenant))
            .ReturnsAsync(expectedTenant);

        //when
        Tenant actualTenant = await tenantProcessingService.UpdateTenantAsync(inputTenant);

        //then
        actualTenant.Should().BeEquivalentTo(expectedTenant);

        tenantServiceMock.Verify(tenantServiceMock =>
            tenantServiceMock.UpdateTenantAsync(inputTenant),
            Times.Once);

        tenantServiceMock.VerifyNoOtherCalls();
    }
}
