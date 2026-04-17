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

        roleBrokerMock.Setup(broker => broker.UpdateSSORoleAsync(inputSSORole)).ReturnsAsync(expectedSSORole);

        // when
        SSORole actualSSORole = await roleService.UpdateSSORoleAsync(inputSSORole);

        // then
        actualSSORole.Should().BeEquivalentTo(expectedSSORole);

        roleBrokerMock.Verify(broker => 
            broker.UpdateSSORoleAsync(inputSSORole), 
            Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}
