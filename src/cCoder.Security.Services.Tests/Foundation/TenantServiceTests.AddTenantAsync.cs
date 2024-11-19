using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TenantServiceTests
{
    [Fact]
    public async void AddTenantAsyncWorksAsExpected()
    {
        //given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();
        DateTimeOffset expectedTime = DateTimeOffset.Now;

        dateTimeOffsetBrokerMock.Setup(dateTimeOffsetBrokerMock =>
            dateTimeOffsetBrokerMock.GetCurrentTime())
            .Returns(expectedTime);

        tenantBrokerMock.Setup(tenantBrokerMock =>
            tenantBrokerMock.AddTenantAsync(inputTenant))
            .ReturnsAsync(inputTenant);

        //when
        Tenant actualTenant = await tenantService.AddTenantAsync(inputTenant);

        //then
        actualTenant.CreatedOn.Should().BeCloseTo(expectedTime, TimeSpan.FromSeconds(1));
        actualTenant.LastUpdated.Should().BeCloseTo(expectedTime, TimeSpan.FromSeconds(1));

        expectedTenant.CreatedOn = actualTenant.CreatedOn;
        expectedTenant.LastUpdated = actualTenant.LastUpdated;

        actualTenant.Should().BeEquivalentTo(expectedTenant);

        tenantBrokerMock.Verify(tenantBrokerMock =>
            tenantBrokerMock.AddTenantAsync(inputTenant), Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}
