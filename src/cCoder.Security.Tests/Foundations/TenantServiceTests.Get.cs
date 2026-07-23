// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TenantServiceTests
{
    [Fact]
    public void GetAllTenantsWorksAsExpected()
    {
        //given
        IQueryable<Tenant> expectedTenants = RandomTenants()
            .AsQueryable();

        tenantBrokerMock.Setup(tenantBrokerMock =>
            tenantBrokerMock.GetAllTenants())
            .Returns(value: expectedTenants);

        //when
        IQueryable<Tenant> actualTenants = tenantService.GetAllTenants();

        //then
        actualTenants.Should().BeEquivalentTo(expectation: expectedTenants);

        tenantBrokerMock.Verify(expression: tenantBrokerMock =>
            tenantBrokerMock.GetAllTenants(),
times: Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}