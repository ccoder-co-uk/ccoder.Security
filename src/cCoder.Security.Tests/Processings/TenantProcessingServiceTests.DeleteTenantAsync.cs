// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class TenantProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteTenantAsync()
    {
        // Given
        Tenant inputTenant = RandomTenant();

        // When
        await tenantProcessingService.DeleteTenantAsync(item: inputTenant);

        // Then
        tenantServiceMock.Verify(expression: tenantServiceMock =>
            tenantServiceMock.DeleteTenantAsync(tenant: inputTenant),
times: Times.Once());

        tenantServiceMock.VerifyNoOtherCalls();
    }
}