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
        // Given
        IQueryable<UserEvent> expectedUserEvents = RandomUserEvents()
            .AsQueryable();

        userEventBrokerMock.Setup(expression: userEventBrokerMock =>
            userEventBrokerMock.SelectAllUserEvents())
            .Returns(value: expectedUserEvents);

        // When
        IQueryable<UserEvent> actualUserEvents = userEventService.GetAllUserEvents();

        // Then
        actualUserEvents.Should()
            .BeEquivalentTo(expectation: expectedUserEvents);

        userEventBrokerMock.Verify(expression: userEventBrokerMock =>
            userEventBrokerMock.SelectAllUserEvents(),
times: Times.Once());

        userEventBrokerMock.VerifyNoOtherCalls();
    }
}