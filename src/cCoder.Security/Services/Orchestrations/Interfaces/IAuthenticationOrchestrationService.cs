using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;
internal interface IAuthenticationOrchestrationService
{
    ValueTask<Token> IssueTokenAsync(string userId);
    ValueTask<Token> LoginAsync(string username, string password);
    ValueTask Logout(string tokenId = null);
    ValueTask<Token> GenerateForgotPasswordToken(string userId);
}

