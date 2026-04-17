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
        //given
        IQueryable<UserEvent> expectedUserEvents = RandomUserEvents()
            .AsQueryable();

        userEventServiceMock.Setup(userEventServiceMock =>
            userEventServiceMock.GetAllUserEvents())
            .Returns(expectedUserEvents);

        //when
        IQueryable<UserEvent> actualUserEvents = userEventProcessingService.GetAllUserEvents();

        //then
        actualUserEvents.Should().BeEquivalentTo(expectedUserEvents);

        userEventServiceMock.Verify(userEventServiceMock =>
            userEventServiceMock.GetAllUserEvents(),
            Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}

