using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSORoleServiceTests
{
    [Fact]
    public void ShouldGetSSORolesAsync()
    {
        // given
        IQueryable<SSORole> expectedSSORoles = RandomRoles();
        roleBrokerMock.Setup(broker => broker.GetAllSSORoles()).Returns(expectedSSORoles);

        // when
        IEnumerable<SSORole> actualSSORoles = roleService.GetAllSSORoles();

        // then
        actualSSORoles.Should().BeEquivalentTo(expectedSSORoles);
        roleBrokerMock.Verify(broker => broker.GetAllSSORoles(), Times.Once);
        roleBrokerMock.VerifyNoOtherCalls();
    }
}