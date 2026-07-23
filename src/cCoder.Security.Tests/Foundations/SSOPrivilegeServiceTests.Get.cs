// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOPrivilegeServiceTests
{
    [Fact]
    public void ShouldGetAllSSOPrivileges()
    {
        // Given
        IQueryable<SSOPrivilege> expectedSSOPrivileges = RandomSSOPrivileges();

        privBrokerMock.Setup(expression: broker => broker.SelectPrivileges())
            .Returns(value: expectedSSOPrivileges);

        // When
        IEnumerable<SSOPrivilege> actualSSOPrivileges = privService.GetAllSSOPrivileges();

        // Then
        actualSSOPrivileges.Should()
            .BeEquivalentTo(expectation: expectedSSOPrivileges);

        privBrokerMock.Verify(expression: broker => broker.SelectPrivileges(), times: Times.Once);
        privBrokerMock.VerifyNoOtherCalls();
    }
}