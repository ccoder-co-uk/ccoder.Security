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
        await Assert.ThrowsAsync<SecurityException>(async() => await authenticationOrchestrationService.LoginAsync(username, password));
    }
}

