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
    public async Task AddTenantAsyncWorksAsExpected()
    {
        //given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();
        DateTimeOffset expectedTime = DateTimeOffset.Now;

        dateTimeOffsetBrokerMock.Setup(expression: dateTimeOffsetBrokerMock =>
            dateTimeOffsetBrokerMock.GetCurrentTime())
            .Returns(value: expectedTime);

        Tenant submitted = null;

        tenantBrokerMock.Setup(tenantBrokerMock =>
            tenantBrokerMock.InsertTenantAsync(It.IsAny<Tenant>()))
            .Callback<Tenant>(action: candidate => submitted = candidate)
            .ReturnsAsync(value: inputTenant);

        //when
        Tenant actualTenant = await tenantService.AddTenantAsync(tenant: inputTenant);

        //then
        actualTenant.Should().BeSameAs(expected: inputTenant);
        submitted.Should().NotBeSameAs(unexpected: inputTenant);
        actualTenant.Should().NotBeSameAs(unexpected: submitted);
        actualTenant.CreatedOn.Should().BeCloseTo(nearbyTime: expectedTime, precision: TimeSpan.FromSeconds(seconds: 1));
        actualTenant.LastUpdated.Should().BeCloseTo(nearbyTime: expectedTime, precision: TimeSpan.FromSeconds(seconds: 1));

        expectedTenant.CreatedOn = actualTenant.CreatedOn;
        expectedTenant.LastUpdated = actualTenant.LastUpdated;

        actualTenant.Should().BeEquivalentTo(expectation: expectedTenant);

        tenantBrokerMock.Verify(expression: tenantBrokerMock =>
            tenantBrokerMock.InsertTenantAsync(tenant: It.IsAny<Tenant>()), times: Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}