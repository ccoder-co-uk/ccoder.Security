// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSORoleServiceTests
{
    [Fact]
    public void ShouldGetSSORolesAsync()
    {
        // given
        IQueryable<SSORole> expectedSSORoles = RandomRoles();
        roleBrokerMock.Setup(expression: broker => broker.GetAllSSORoles()).Returns(value: expectedSSORoles);

        // when
        IEnumerable<SSORole> actualSSORoles = roleService.GetAllSSORoles();

        // then
        actualSSORoles.Should().BeEquivalentTo(expectation: expectedSSORoles);
        roleBrokerMock.Verify(expression: broker => broker.GetAllSSORoles(), times: Times.Once);
        roleBrokerMock.VerifyNoOtherCalls();
    }
}