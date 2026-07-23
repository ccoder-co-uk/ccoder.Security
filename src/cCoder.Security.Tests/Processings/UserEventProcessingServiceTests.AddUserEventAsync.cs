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
        //given
        UserEvent inputUserEvent = RandomUserEvent();
        UserEvent expectedUserEvent = inputUserEvent.DeepClone();

        userEventServiceMock.Setup(userEventServiceMock =>
            userEventServiceMock.AddUserEventAsync(inputUserEvent))
            .ReturnsAsync(value: expectedUserEvent);

        //when
        UserEvent actualUserEvent = await userEventProcessingService.AddUserEventAsync(userEvent: inputUserEvent);

        //then
        actualUserEvent.Should().BeEquivalentTo(expectation: expectedUserEvent);

        userEventServiceMock.Verify(expression: userEventServiceMock =>
            userEventServiceMock.AddUserEventAsync(inputUserEvent),
times: Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}