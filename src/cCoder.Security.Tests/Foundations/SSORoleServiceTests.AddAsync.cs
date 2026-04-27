using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSORoleServiceTests
{
    [Fact]
    public async Task ShouldAddSSORoleAsync()
    {
        // given
        SSORole inputSSORole = RandomRole(Guid.NewGuid());
        SSORole expectedSSORole = inputSSORole.DeepClone();

        roleBrokerMock.Setup(broker => broker.GetAllSSORoles()).Returns(Array.Empty<SSORole>().AsQueryable());
        SSORole submitted = null;
        roleBrokerMock
            .Setup(broker => broker.AddSSORoleAsync(It.IsAny<SSORole>()))
            .Callback<SSORole>(candidate => submitted = candidate)
            .ReturnsAsync(expectedSSORole);

        // when
        SSORole actualSSORole = await roleService.AddSSORoleAsync(inputSSORole);

        // then
        actualSSORole.Should().BeSameAs(inputSSORole);
        submitted.Should().NotBeSameAs(inputSSORole);
        actualSSORole.Should().NotBeSameAs(submitted);
        actualSSORole.Should().BeEquivalentTo(expectedSSORole);

        roleBrokerMock.Verify(broker => 
            broker.AddSSORoleAsync(It.IsAny<SSORole>()), 
            Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}
