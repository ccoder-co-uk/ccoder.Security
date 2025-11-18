using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

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
            .ReturnsAsync(expectedUserEvent);

        //when
        UserEvent actualUserEvent = await userEventProcessingService.AddUserEventAsync(inputUserEvent);

        //then
        actualUserEvent.Should().BeEquivalentTo(expectedUserEvent);

        userEventServiceMock.Verify(userEventServiceMock =>
            userEventServiceMock.AddUserEventAsync(inputUserEvent),
            Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}
