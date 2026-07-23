// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class UserEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteUserEventAsync()
    {
        //given
        UserEvent inputUserEvent = RandomUserEvent();

        //when
        await userEventProcessingService.DeleteUserEventAsync(deletedUserEvent: inputUserEvent);

        //then
        userEventServiceMock.Verify(expression: userEventServiceMock =>
            userEventServiceMock.DeleteUserEventAsync(deletedUserEvent: inputUserEvent),
times: Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}