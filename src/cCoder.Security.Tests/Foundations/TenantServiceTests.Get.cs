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
        // Given
        IQueryable<Tenant> expectedTenants = RandomTenants()
            .AsQueryable();

        tenantBrokerMock.Setup(expression: tenantBrokerMock =>
            tenantBrokerMock.SelectAllTenants())
            .Returns(value: expectedTenants);

        // When
        IQueryable<Tenant> actualTenants = tenantService.GetAllTenants();

        // Then
        actualTenants.Should()
            .BeEquivalentTo(expectation: expectedTenants);

        tenantBrokerMock.Verify(expression: tenantBrokerMock =>
            tenantBrokerMock.SelectAllTenants(),
times: Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}