// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserServiceTests
{
    [Fact]
    public void ShouldGetSSOUsersAsync()
    {
        // given
        IQueryable<SSOUser> expectedSSOUsers = RandomUsers();
        userBrokerMock.Setup(expression: broker => broker.GetAllSSOUsers(false)).Returns(value: expectedSSOUsers);

        // when
        IEnumerable<SSOUser> actualSSOUsers = userService.GetAllSSOUsers();

        // then
        actualSSOUsers.Should().BeEquivalentTo(expectation: expectedSSOUsers);
        userBrokerMock.Verify(expression: broker => broker.GetAllSSOUsers(ignoreFilters: false), times: Times.Once);
        userBrokerMock.VerifyNoOtherCalls();
    }
}