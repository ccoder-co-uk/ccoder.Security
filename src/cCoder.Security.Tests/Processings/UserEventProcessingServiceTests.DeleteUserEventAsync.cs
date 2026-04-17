using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class UserEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteUserEventAsync()
    {
        //given
        UserEvent inputUserEvent = RandomUserEvent();

        //when
        await userEventProcessingService.DeleteUserEventAsync(inputUserEvent);

        //then
        userEventServiceMock.Verify(userEventServiceMock =>
            userEventServiceMock.DeleteUserEventAsync(inputUserEvent),
            Times.Once());

        userEventServiceMock.VerifyNoOtherCalls();
    }
}

