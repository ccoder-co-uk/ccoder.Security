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
        //given
        Tenant inputTenant = RandomTenant();

        //when
        await tenantProcessingService.DeleteTenantAsync(deletedTenant: inputTenant);

        //then
        tenantServiceMock.Verify(expression: tenantServiceMock =>
            tenantServiceMock.DeleteTenantAsync(deletedTenant: inputTenant),
times: Times.Once());

        tenantServiceMock.VerifyNoOtherCalls();
    }
}