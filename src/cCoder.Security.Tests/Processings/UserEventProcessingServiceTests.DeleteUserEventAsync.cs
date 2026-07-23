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
        // Given
        UserEvent inputUserEvent = RandomUserEvent();

        // When
        await userEventProcessingService.DeleteUserEventAsync(userEvent: inputUserEvent);

        // Then
        userEventServiceMock.Verify(expression: userEventServiceMock =>
            userEventServiceMock.DeleteUserEventAsync(userEvent: inputUserEvent),
times: Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}