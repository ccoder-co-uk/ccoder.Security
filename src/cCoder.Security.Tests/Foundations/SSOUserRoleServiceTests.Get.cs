// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserRoleServiceTests
{
    [Fact]
    public void ShouldGetSSOUserRolesAsync()
    {
        // given
        IQueryable<SSOUserRole> expectedSSOUserRoles = RandomUserRoles();
        userRoleBrokerMock.Setup(expression: broker => broker.GetAllSSOUserRoles()).Returns(value: expectedSSOUserRoles);

        // when
        IEnumerable<SSOUserRole> actualSSOUserRoles = userRoleService.GetAllSSOUserRoles();

        // then
        actualSSOUserRoles.Should().BeEquivalentTo(expectation: expectedSSOUserRoles);
        userRoleBrokerMock.Verify(expression: broker => broker.GetAllSSOUserRoles(), times: Times.Once);
        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}