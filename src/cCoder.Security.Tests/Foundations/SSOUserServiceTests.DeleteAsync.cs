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
        // given
        SSOUser inputSSOUser = RandomUser(id: RandomString());
        SSOUser expectedSSOUser = inputSSOUser.DeepClone();

        // when
        await userService.DeleteSSOUserAsync(item: inputSSOUser);

        // then
        userBrokerMock.Verify(expression: broker =>
            broker.DeleteSSOUserAsync(SSOUser: inputSSOUser),
times: Times.Once);

        userBrokerMock.VerifyNoOtherCalls();
    }
}