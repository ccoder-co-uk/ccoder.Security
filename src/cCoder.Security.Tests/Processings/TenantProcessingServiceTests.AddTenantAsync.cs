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
    public async Task ShouldAddTenantAsync()
    {
        // Given
        Tenant inputTenant = RandomTenant();
        Tenant expectedTenant = inputTenant.DeepClone();

        tenantServiceMock.Setup(expression: tenantServiceMock =>
            tenantServiceMock.AddTenantAsync(tenant:inputTenant))
            .ReturnsAsync(value: expectedTenant);

        // When
        Tenant actualTenant = await tenantProcessingService.AddTenantAsync(item: inputTenant);

        // Then
        actualTenant.Should()
            .BeEquivalentTo(expectation: expectedTenant);

        tenantServiceMock.Verify(expression: tenantServiceMock =>
            tenantServiceMock.AddTenantAsync(tenant: inputTenant),
times: Times.Once());

        tenantServiceMock.VerifyNoOtherCalls();
    }
}