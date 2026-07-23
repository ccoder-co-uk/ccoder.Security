// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class UserEventServiceTests
{
    [Fact]
    public async Task DeleteUserEventAsyncWorksAsExpected()
    {
        //given
        UserEvent inputUserEvent = RandomUserEvent();

        //when
        await userEventService.DeleteUserEventAsync(userEvent: inputUserEvent);

        //then
        userEventBrokerMock.Verify(expression: userEventBrokerMock =>
            userEventBrokerMock.DeleteUserEventAsync(userEvent: inputUserEvent),
times: Times.Once());

        userEventBrokerMock.VerifyNoOtherCalls();
    }
}