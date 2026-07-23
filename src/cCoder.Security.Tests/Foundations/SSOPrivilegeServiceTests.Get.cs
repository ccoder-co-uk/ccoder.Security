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
        // given
        IQueryable<SSOPrivilege> expectedSSOPrivileges = RandomSSOPrivileges();
        privBrokerMock.Setup(broker => broker.GetPrivileges()).Returns(value: expectedSSOPrivileges);

        // when
        IEnumerable<SSOPrivilege> actualSSOPrivileges = privService.GetAllSSOPrivileges();

        // then
        actualSSOPrivileges.Should().BeEquivalentTo(expectation: expectedSSOPrivileges);
        privBrokerMock.Verify(expression: broker => broker.GetPrivileges(), times: Times.Once);
        privBrokerMock.VerifyNoOtherCalls();
    }
}