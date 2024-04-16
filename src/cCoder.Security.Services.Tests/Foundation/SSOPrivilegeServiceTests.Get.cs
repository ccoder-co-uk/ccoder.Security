using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using cCoder.Security.Objects.Entities;
using Xunit;

namespace cCoder.Security.Services.Tests.Foundation
{
    public partial class SSOPrivilegeServiceTests
    {
        [Fact]
        public void ShouldGetAllSSOPrivileges()
        {
            // given
            IQueryable<SSOPrivilege> expectedSSOPrivileges = RandomSSOPrivileges();
            privBrokerMock.Setup(broker => broker.GetPrivileges()).Returns(expectedSSOPrivileges);

            // when
            IEnumerable<SSOPrivilege> actualSSOPrivileges = privService.GetAllSSOPrivileges();

            // then
            actualSSOPrivileges.Should().BeEquivalentTo(expectedSSOPrivileges);
            privBrokerMock.Verify(broker => broker.GetPrivileges(), Times.Once);
            privBrokerMock.VerifyNoOtherCalls();
        }
    }
}