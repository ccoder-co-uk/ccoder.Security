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
        // Given
        SSOUserRole inputSSOUserRole = RandomUserRole();
        SSOUserRole expectedSSOUserRole = inputSSOUserRole.DeepClone();

        // When
        await userRoleService.DeleteSSOUserRoleAsync(item: inputSSOUserRole);

        // Then
        userRoleBrokerMock.Verify(expression: broker =>
            broker.DeleteSSOUserRoleAsync(userRole: inputSSOUserRole),
times: Times.Once);

        userRoleBrokerMock.VerifyNoOtherCalls();
    }
}