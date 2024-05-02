using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSOUserServiceTests
{
    [Fact]
    public void ShouldGetSSOUsersAsync()
    {
        // given
        IQueryable<SSOUser> expectedSSOUsers = RandomUsers();
        userBrokerMock.Setup(broker => broker.GetAllSSOUsers(false)).Returns(expectedSSOUsers);

        // when
        IEnumerable<SSOUser> actualSSOUsers = userService.GetAllSSOUsers();

        // then
        actualSSOUsers.Should().BeEquivalentTo(expectedSSOUsers);
        userBrokerMock.Verify(broker => broker.GetAllSSOUsers(false), Times.Once);
        userBrokerMock.VerifyNoOtherCalls();
    }
}