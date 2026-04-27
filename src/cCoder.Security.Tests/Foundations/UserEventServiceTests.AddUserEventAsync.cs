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
            .ReturnsAsync(inputUserEvent);

        dateTimeOffsetBrokerMock.Setup(dateTimeOffsetBrokerMock =>
            dateTimeOffsetBrokerMock.GetCurrentTime())
            .Returns(expectedTime);

        //when
        UserEvent actualUserEvent = await userEventService.AddUserEventAsync(inputUserEvent);

        //then
        actualUserEvent.Should().BeSameAs(inputUserEvent);
        submitted.Should().NotBeSameAs(inputUserEvent);
        actualUserEvent.Should().NotBeSameAs(submitted);
        actualUserEvent.Should().BeEquivalentTo(expectedUserEvent);

        userEventBrokerMock.Verify(userEventBrokerMock =>
            userEventBrokerMock.AddUserEventAsync(It.IsAny<UserEvent>()),
            Times.Once());

        userEventBrokerMock.VerifyNoOtherCalls();
    }
}

