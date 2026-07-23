// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class UserEventProcessingServiceTests
{
    [Fact]
    public void ShouldGetAllUserEvents()
    {
        // Given
        IQueryable<UserEvent> expectedUserEvents = RandomUserEvents()
            .AsQueryable();

        userEventServiceMock.Setup(expression: userEventServiceMock =>
            userEventServiceMock.GetAllUserEvents())
            .Returns(value: expectedUserEvents);

        // When
        IQueryable<UserEvent> actualUserEvents = userEventProcessingService.GetAllUserEvents();

        // Then
        actualUserEvents.Should()
            .BeEquivalentTo(expectation: expectedUserEvents);

        userEventServiceMock.Verify(expression: userEventServiceMock =>
            userEventServiceMock.GetAllUserEvents(),
times: Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}