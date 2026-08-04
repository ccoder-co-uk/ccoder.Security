// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.EventHandlers;
using cCoder.Security.Services.Foundations.Events;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Exposures;

public sealed partial class SecurityEventHandlersTests
{
    [Fact]
    public void ShouldDelegateListeningToAllEvents()
    {
        // Given
        var eventHandlerServiceMock =
            new Mock<IEventHandlerService>(behavior: MockBehavior.Strict);

        eventHandlerServiceMock
            .Setup(expression: service => service.ListenToAllEvents());

        var eventHandlers = new SecurityEventHandlers(
            eventHandlerService: eventHandlerServiceMock.Object);

        // When
        eventHandlers.ListenToAllEvents();

        // Then
        eventHandlerServiceMock.Verify(
            expression: service => service.ListenToAllEvents(),
            times: Times.Once);

        eventHandlerServiceMock.VerifyNoOtherCalls();
    }
}