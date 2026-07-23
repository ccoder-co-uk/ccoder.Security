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
        // Given
        UserEvent inputUserEvent = RandomUserEvent();

        // When
        await userEventService.DeleteUserEventAsync(userEvent: inputUserEvent);

        // Then
        userEventBrokerMock.Verify(expression: userEventBrokerMock =>
            userEventBrokerMock.DeleteUserEventAsync(userEvent: inputUserEvent),
times: Times.Once());

        userEventBrokerMock.VerifyNoOtherCalls();
    }
}