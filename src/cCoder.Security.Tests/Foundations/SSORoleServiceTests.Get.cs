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
        // Given
        IQueryable<SSORole> expectedSSORoles = RandomRoles();

        roleBrokerMock.Setup(expression: broker => broker.SelectAllSSORoles())
            .Returns(value: expectedSSORoles);

        // When
        IEnumerable<SSORole> actualSSORoles = roleService.GetAllSSORoles();

        // Then
        actualSSORoles.Should()
            .BeEquivalentTo(expectation: expectedSSORoles);

        roleBrokerMock.Verify(expression: broker => broker.SelectAllSSORoles(), times: Times.Once);
        roleBrokerMock.VerifyNoOtherCalls();
    }
}