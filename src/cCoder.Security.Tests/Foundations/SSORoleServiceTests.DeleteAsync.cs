// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public partial class SSORoleServiceTests
{
    [Fact]
    public async Task ShouldDeleteSSORoleAsync()
    {
        // Given
        SSORole inputSSORole = RandomRole(id: Guid.NewGuid());
        SSORole expectedSSORole = inputSSORole.DeepClone();

        // When
        await roleService.DeleteSSORoleAsync(item: inputSSORole);

        // Then
        roleBrokerMock.Verify(expression: broker =>
            broker.DeleteSSORoleAsync(SSORole: inputSSORole),
times: Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}