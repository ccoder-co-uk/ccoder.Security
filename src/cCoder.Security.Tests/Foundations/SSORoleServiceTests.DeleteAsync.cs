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
        // given
        SSORole inputSSORole = RandomRole(id: Guid.NewGuid());
        SSORole expectedSSORole = inputSSORole.DeepClone();

        // when
        await roleService.DeleteSSORoleAsync(item: inputSSORole);

        // then
        roleBrokerMock.Verify(expression: broker =>
            broker.DeleteSSORoleAsync(inputSSORole),
times: Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }
}