// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class TenantProcessingServiceTests
{
    [Fact]
    public async Task ShouldUpdateTenantAsync()
    {
        //given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();

        tenantServiceMock.Setup(expression: tenantServiceMock =>
            tenantServiceMock.UpdateTenantAsync(inputTenant))
            .ReturnsAsync(value: expectedTenant);

        //when
        Tenant actualTenant = await tenantProcessingService.UpdateTenantAsync(item: inputTenant);

        //then
        actualTenant.Should().BeEquivalentTo(expectation: expectedTenant);

        tenantServiceMock.Verify(expression: tenantServiceMock =>
            tenantServiceMock.UpdateTenantAsync(tenant: inputTenant),
times: Times.Once);

        tenantServiceMock.VerifyNoOtherCalls();
    }
}