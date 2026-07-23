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
    public async Task ShouldUpdateSSORoleAsync()
    {
        // given
        SSORole inputSSORole = RandomRole(id: Guid.NewGuid());
        SSORole expectedSSORole = inputSSORole.DeepClone();

        SSORole submitted = null;

        roleBrokerMock
            .Setup(broker => broker.UpdateSSORoleAsync(It.IsAny<SSORole>()))
            .Callback<SSORole>(candidate => submitted = candidate)
            .ReturnsAsync(value: expectedSSORole);

        // when
        SSORole actualSSORole = await roleService.UpdateSSORoleAsync(item: inputSSORole);

        // then
        actualSSORole.Should().BeSameAs(expected: inputSSORole);
        submitted.Should().NotBeSameAs(unexpected: inputSSORole);
        actualSSORole.Should().NotBeSameAs(unexpected: submitted);
        actualSSORole.Should().BeEquivalentTo(expectation: expectedSSORole);

        roleBrokerMock.Verify(expression: broker =>
            broker.UpdateSSORoleAsync(It.IsAny<SSORole>()),
times: Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}