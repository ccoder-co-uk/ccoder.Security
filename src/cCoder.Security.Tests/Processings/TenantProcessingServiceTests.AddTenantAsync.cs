using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class TenantProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddTenantAsync()
    {
        //given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();

        tenantServiceMock.Setup(tenantServiceMock =>
            tenantServiceMock.AddTenantAsync(inputTenant))
            .ReturnsAsync(expectedTenant);

        //when
        Tenant actualTenant = await tenantProcessingService.AddTenantAsync(inputTenant);

        //then
        actualTenant.Should().BeEquivalentTo(expectedTenant);

        tenantServiceMock.Verify(tenantServiceMock =>
            tenantServiceMock.AddTenantAsync(inputTenant),
            Times.Once());

        tenantServiceMock.VerifyNoOtherCalls();
    }
}

