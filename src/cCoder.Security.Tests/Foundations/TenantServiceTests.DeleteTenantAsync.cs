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
        //given
        Tenant inputTenant = RandomTenant();

        //when
        await tenantService.DeleteTenantAsync(tenant: inputTenant);

        //then
        tenantBrokerMock.Verify(expression: tenantBrokerMock =>
            tenantBrokerMock.DeleteTenantAsync(tenant: inputTenant),
times: Times.Once());

        tenantBrokerMock.VerifyNoOtherCalls();
    }
}