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
        // Given
        IQueryable<SSOUser> expectedSSOUsers = RandomUsers();

        userBrokerMock
            .Setup(expression: broker => broker.SelectAllSSOUsers())
            .Returns(value: expectedSSOUsers);

        // When
        IEnumerable<SSOUser> actualSSOUsers = userService.GetAllSSOUsers();

        // Then
        actualSSOUsers.Should()
            .BeEquivalentTo(expectation: expectedSSOUsers);

        userBrokerMock.Verify(
            expression: broker => broker.SelectAllSSOUsers(),
            times: Times.Once);

        userBrokerMock.VerifyNoOtherCalls();
    }
}