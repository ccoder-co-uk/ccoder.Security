using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSORoleServiceTests
{
    [Fact]
    public async Task ShouldUpdateSSORoleAsync()
    {
        // given
        SSORole inputSSORole = RandomRole(Guid.NewGuid());
        SSORole expectedSSORole = inputSSORole.DeepClone();

        SSORole submitted = null;
        roleBrokerMock
            .Setup(broker => broker.UpdateSSORoleAsync(It.IsAny<SSORole>()))
            .Callback<SSORole>(candidate => submitted = candidate)
            .ReturnsAsync(expectedSSORole);

        // when
        SSORole actualSSORole = await roleService.UpdateSSORoleAsync(inputSSORole);

        // then
        actualSSORole.Should().BeSameAs(inputSSORole);
        submitted.Should().NotBeSameAs(inputSSORole);
        actualSSORole.Should().NotBeSameAs(submitted);
        actualSSORole.Should().BeEquivalentTo(expectedSSORole);

        roleBrokerMock.Verify(broker => 
            broker.UpdateSSORoleAsync(It.IsAny<SSORole>()), 
            Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}
