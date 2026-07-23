// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserRoleServiceTests
{
    [Fact]
    public async Task ShouldDeleteSSOUserRoleAsync()
    {
        // given
        SSOUserRole inputSSOUserRole = RandomUserRole();
        SSOUserRole expectedSSOUserRole = inputSSOUserRole.DeepClone();

        // when
        await userRoleService.DeleteSSOUserRoleAsync(item: inputSSOUserRole);

        // then
        userRoleBrokerMock.Verify(expression: broker =>
            broker.DeleteSSOUserRoleAsync(inputSSOUserRole),
times: Times.Once);

        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}