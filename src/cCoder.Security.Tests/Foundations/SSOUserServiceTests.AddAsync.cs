// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserServiceTests
{
    [Fact]
    public async Task ShouldAddSSOUserAsync()
    {
        // Given
        SSOUser inputSSOUser = RandomUser(id: RandomString());
        SSOUser expectedSSOUser = inputSSOUser.DeepClone();

        SSOUser submitted = null;

        userBrokerMock
            .Setup(expression:broker => broker.InsertSSOUserAsync(user:It.IsAny<SSOUser>()))
            .Callback<SSOUser>(action: candidate => submitted = candidate)
            .ReturnsAsync(value: expectedSSOUser);

        // When
        SSOUser actualSSOUser = await userService.AddSSOUserAsync(item: inputSSOUser);

        // Then
        actualSSOUser.Should()
            .BeSameAs(expected: inputSSOUser);

        submitted.Should()
            .NotBeSameAs(unexpected: inputSSOUser);

        actualSSOUser.Should()
            .NotBeSameAs(unexpected: submitted);

        actualSSOUser.Should()
            .BeEquivalentTo(expectation: expectedSSOUser);

        userBrokerMock.Verify(expression: broker => broker.InsertSSOUserAsync(user: It.IsAny<SSOUser>()), times: Times.Once);
        userBrokerMock.VerifyNoOtherCalls();
    }
}