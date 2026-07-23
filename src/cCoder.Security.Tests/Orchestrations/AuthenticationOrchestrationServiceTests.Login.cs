// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using Xunit;

namespace cCoder.Security.Tests.Orchestrations;

public partial class AuthenticationOrchestrationServiceTests
{
    [Fact]
    public async Task LoginThrowsSecurityExceptionIfNotValidLogin()
    {
        //given
        string username = RandomString();
        string password = RandomString();

        //when & then
        await Assert.ThrowsAsync<SecurityException>(testCode: async () => await authenticationOrchestrationService.LoginAsync(username, password));
    }
}