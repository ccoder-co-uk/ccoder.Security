using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class UserEventServiceTests
{
    [Fact]
    public void GetAllUserEventsWorksAsExpected()
    {
        //given
        IQueryable<UserEvent> expectedUserEvents = RandomUserEvents()
            .AsQueryable();

        userEventBrokerMock.Setup(userEventBrokerMock =>
            userEventBrokerMock.GetAllUserEvents())
            .Returns(expectedUserEvents);

        //when
        IQueryable<UserEvent> actualUserEvents = userEventService.GetAllUserEvents();

        //then
        actualUserEvents.Should().BeEquivalentTo(expectedUserEvents);

        userEventBrokerMock.Verify(userEventBrokerMock =>
            userEventBrokerMock.GetAllUserEvents(),
            Times.Once());

        userEventBrokerMock.VerifyNoOtherCalls();
    }
}

