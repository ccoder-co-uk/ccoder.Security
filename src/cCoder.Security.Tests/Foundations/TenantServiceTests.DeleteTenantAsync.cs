// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class TenantServiceTests
{
    [Fact]
    public async Task DeleteTenantAsyncWorksAsExpected()
    {
        // Given
        Tenant inputTenant = RandomTenant();

        // When
        await tenantService.DeleteTenantAsync(tenant: inputTenant);

        // Then
        tenantBrokerMock.Verify(expression: tenantBrokerMock =>
            tenantBrokerMock.DeleteTenantAsync(tenant: inputTenant),
times: Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}