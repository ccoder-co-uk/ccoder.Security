// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserServiceTests
{
    [Fact]
    public async Task ShouldDeleteSSOUserAsync()
    {
        // Given
        SSOUser inputSSOUser = RandomUser(id: RandomString());
        SSOUser expectedSSOUser = inputSSOUser.DeepClone();

        // When
        await userService.DeleteSSOUserAsync(item: inputSSOUser);

        // Then
        userBrokerMock.Verify(expression: broker =>
            broker.DeleteSSOUserAsync(SSOUser: inputSSOUser),
times: Times.Once);

        userBrokerMock.VerifyNoOtherCalls();
    }
}