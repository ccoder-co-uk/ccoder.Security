// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        SSORole inputSSORole = RandomRole(id: Guid.NewGuid());
        SSORole expectedSSORole = inputSSORole.DeepClone();

        roleBrokerMock.Setup(broker => broker.GetAllSSORoles()).Returns(value: Array.Empty<SSORole>().AsQueryable());
        SSORole submitted = null;

        roleBrokerMock
            .Setup(broker => broker.AddSSORoleAsync(It.IsAny<SSORole>()))
            .Callback<SSORole>(candidate => submitted = candidate)
            .ReturnsAsync(value: expectedSSORole);

        // when
        SSORole actualSSORole = await roleService.AddSSORoleAsync(item: inputSSORole);

        // then
        actualSSORole.Should().BeSameAs(expected: inputSSORole);
        submitted.Should().NotBeSameAs(unexpected: inputSSORole);
        actualSSORole.Should().NotBeSameAs(unexpected: submitted);
        actualSSORole.Should().BeEquivalentTo(expectation: expectedSSORole);

        roleBrokerMock.Verify(expression: broker =>
            broker.AddSSORoleAsync(It.IsAny<SSORole>()),
times: Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}