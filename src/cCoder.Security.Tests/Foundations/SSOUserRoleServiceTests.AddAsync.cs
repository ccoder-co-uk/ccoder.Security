// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserRoleServiceTests
{
    [Fact]
    public async Task ShouldAddSSOUserRoleAsync()
    {
        // given
        SSOUserRole inputSSOUserRole = RandomUserRole();
        SSOUserRole expectedSSOUserRole = inputSSOUserRole.DeepClone();

        userRoleBrokerMock.Setup(expression: broker => broker.SelectAllSSOUserRoles()).Returns(value: Array.Empty<SSOUserRole>().AsQueryable());
        SSOUserRole submitted = null;

        userRoleBrokerMock
            .Setup(broker => broker.InsertSSOUserRoleAsync(It.IsAny<SSOUserRole>()))
            .Callback<SSOUserRole>(action: candidate => submitted = candidate)
            .ReturnsAsync(value: expectedSSOUserRole);

        // when
        SSOUserRole actualSSOUserRole = await userRoleService.AddSSOUserRoleAsync(item: inputSSOUserRole);

        // then
        actualSSOUserRole.Should().BeSameAs(expected: inputSSOUserRole);
        submitted.Should().NotBeSameAs(unexpected: inputSSOUserRole);
        actualSSOUserRole.Should().NotBeSameAs(unexpected: submitted);
        actualSSOUserRole.Should().BeEquivalentTo(expectation: expectedSSOUserRole);

        userRoleBrokerMock.Verify(expression: broker =>
            broker.InsertSSOUserRoleAsync(userRole: It.IsAny<SSOUserRole>()),
times: Times.Once);

        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}