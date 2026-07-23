// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TenantServiceTests
{
    [Fact]
    public async Task UpdateTenantAsyncWorksAsExpected()
    {
        //given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();
        DateTimeOffset expectedTime = DateTimeOffset.Now;

        dateTimeOffsetBrokerMock.Setup(dateTimeOffsetBrokerMock =>
            dateTimeOffsetBrokerMock.GetCurrentTime())
            .Returns(value: expectedTime);

        Tenant submitted = null;
        tenantBrokerMock.Setup(tenantBrokerMock =>
            tenantBrokerMock.UpdateTenantAsync(It.IsAny<Tenant>()))
            .Callback<Tenant>(candidate => submitted = candidate)
            .ReturnsAsync(value: inputTenant);

        //when
        Tenant actualTenant = await tenantService.UpdateTenantAsync(tenant: inputTenant);

        //then
        actualTenant.Should().BeSameAs(expected: inputTenant);
        submitted.Should().NotBeSameAs(unexpected: inputTenant);
        actualTenant.Should().NotBeSameAs(unexpected: submitted);
        actualTenant.LastUpdated.Should().BeCloseTo(nearbyTime: expectedTime, precision: TimeSpan.FromSeconds(1));
        expectedTenant.LastUpdated = actualTenant.LastUpdated;

        actualTenant.Should().BeEquivalentTo(expectation: expectedTenant);

        tenantBrokerMock.Verify(expression: tenantBrokerMock =>
            tenantBrokerMock.UpdateTenantAsync(It.IsAny<Tenant>()),
times: Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}