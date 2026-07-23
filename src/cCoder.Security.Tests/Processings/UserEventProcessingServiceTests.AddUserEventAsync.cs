// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class UserEventProcessingServiceTests
{
    [Fact]
    public async Task AddUserEventAsyncWorksAsExpected()
    {
        // Given
        UserEvent inputUserEvent = RandomUserEvent();
        UserEvent expectedUserEvent = inputUserEvent.DeepClone();

        userEventServiceMock.Setup(expression: userEventServiceMock =>
            userEventServiceMock.AddUserEventAsync(userEvent:inputUserEvent))
            .ReturnsAsync(value: expectedUserEvent);

        // When
        UserEvent actualUserEvent = await userEventProcessingService.AddUserEventAsync(userEvent: inputUserEvent);

        // Then
        actualUserEvent.Should()
            .BeEquivalentTo(expectation: expectedUserEvent);

        userEventServiceMock.Verify(expression: userEventServiceMock =>
            userEventServiceMock.AddUserEventAsync(userEvent: inputUserEvent),
times: Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}