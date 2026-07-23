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
    public void ShouldGetTenants()
    {
        //given
        IQueryable<Tenant> expectedTenants = RandomTenants()
            .AsQueryable();

        tenantServiceMock.Setup(expression: tenantServiceMock =>
            tenantServiceMock.GetAllTenants())
            .Returns(value: expectedTenants);

        //when
        IQueryable<Tenant> actualTenants = tenantProcessingService.GetAllTenants();

        //then
        tenantServiceMock.Verify(expression: tenantServiceMock =>
            tenantServiceMock.GetAllTenants(),
times: Times.Once());

        tenantServiceMock.VerifyNoOtherCalls();
    }
}