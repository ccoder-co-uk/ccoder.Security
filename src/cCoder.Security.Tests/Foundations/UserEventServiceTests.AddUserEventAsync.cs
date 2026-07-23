// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class UserEventServiceTests
{
    [Fact]
    public async Task AddUserEventAsyncWorksAsExpected()
    {
        //given
        UserEvent inputUserEvent = RandomUserEvent();
        UserEvent expectedUserEvent = inputUserEvent.DeepClone();
        DateTimeOffset expectedTime = DateTimeOffset.Now;

        expectedUserEvent.CreatedOn = expectedTime;

        UserEvent submitted = null;

        userEventBrokerMock.Setup(userEventBrokerMock =>
            userEventBrokerMock.AddUserEventAsync(It.IsAny<UserEvent>()))
            .Callback<UserEvent>(candidate => submitted = candidate)
            .ReturnsAsync(value: inputUserEvent);

        dateTimeOffsetBrokerMock.Setup(dateTimeOffsetBrokerMock =>
            dateTimeOffsetBrokerMock.GetCurrentTime())
            .Returns(value: expectedTime);

        //when
        UserEvent actualUserEvent = await userEventService.AddUserEventAsync(userEvent: inputUserEvent);

        //then
        actualUserEvent.Should().BeSameAs(expected: inputUserEvent);
        submitted.Should().NotBeSameAs(unexpected: inputUserEvent);
        actualUserEvent.Should().NotBeSameAs(unexpected: submitted);
        actualUserEvent.Should().BeEquivalentTo(expectation: expectedUserEvent);

        userEventBrokerMock.Verify(expression: userEventBrokerMock =>
            userEventBrokerMock.AddUserEventAsync(It.IsAny<UserEvent>()),
times: Times.Once());

        userEventBrokerMock.VerifyNoOtherCalls();
    }
}