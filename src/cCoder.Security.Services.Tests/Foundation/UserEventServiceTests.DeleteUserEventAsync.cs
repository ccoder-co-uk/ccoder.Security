using Moq;
using cCoder.Security.Objects.Entities;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation
{
    public partial class UserEventServiceTests
    {
        [Fact]
        public async void DeleteUserEventAsyncWorksAsExpected()
        {
            //given
            UserEvent inputUserEvent = RandomUserEvent();

            //when
            await userEventService.DeleteUserEventAsync(inputUserEvent);

            //then
            userEventBrokerMock.Verify(userEventBrokerMock =>
                userEventBrokerMock.DeleteUserEventAsync(inputUserEvent),
                Times.Once());

            userEventBrokerMock.VerifyNoOtherCalls();
        }
    }
}
