using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TenantServiceTests
{
    [Fact]
    public async Task AddTenantAsyncWorksAsExpected()
    {
        //given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();
        DateTimeOffset expectedTime = DateTimeOffset.Now;

        dateTimeOffsetBrokerMock.Setup(dateTimeOffsetBrokerMock =>
            dateTimeOffsetBrokerMock.GetCurrentTime())
            .Returns(expectedTime);

        Tenant submitted = null;
        tenantBrokerMock.Setup(tenantBrokerMock =>
            tenantBrokerMock.AddTenantAsync(It.IsAny<Tenant>()))
            .Callback<Tenant>(candidate => submitted = candidate)
            .ReturnsAsync(inputTenant);

        //when
        Tenant actualTenant = await tenantService.AddTenantAsync(inputTenant);

        //then
        actualTenant.Should().BeSameAs(inputTenant);
        submitted.Should().NotBeSameAs(inputTenant);
        actualTenant.Should().NotBeSameAs(submitted);
        actualTenant.CreatedOn.Should().BeCloseTo(expectedTime, TimeSpan.FromSeconds(1));
        actualTenant.LastUpdated.Should().BeCloseTo(expectedTime, TimeSpan.FromSeconds(1));

        expectedTenant.CreatedOn = actualTenant.CreatedOn;
        expectedTenant.LastUpdated = actualTenant.LastUpdated;

        actualTenant.Should().BeEquivalentTo(expectedTenant);

        tenantBrokerMock.Verify(tenantBrokerMock =>
            tenantBrokerMock.AddTenantAsync(It.IsAny<Tenant>()), Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}

