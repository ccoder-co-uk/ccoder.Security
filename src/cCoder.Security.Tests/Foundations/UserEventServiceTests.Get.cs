// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        userEventBrokerMock.Setup(expression: userEventBrokerMock =>
            userEventBrokerMock.SelectAllUserEvents())
            .Returns(value: expectedUserEvents);

        //when
        IQueryable<UserEvent> actualUserEvents = userEventService.GetAllUserEvents();

        //then
        actualUserEvents.Should().BeEquivalentTo(expectation: expectedUserEvents);

        userEventBrokerMock.Verify(expression: userEventBrokerMock =>
            userEventBrokerMock.SelectAllUserEvents(),
times: Times.Once());

        userEventBrokerMock.VerifyNoOtherCalls();
    }
}