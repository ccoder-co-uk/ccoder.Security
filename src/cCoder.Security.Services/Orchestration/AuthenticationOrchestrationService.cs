using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using System.Security;

namespace cCoder.Security.Services.Orchestration;

public class AuthenticationOrchestrationService 
    : IAuthenticationOrchestrationService
{
    private readonly ISSOUserProcessingService ssoUserProcessingService;
    private readonly ITokenProcessingService tokenProcessingService;
    private readonly ISessionProcessingService sessionProcessingService;

    public AuthenticationOrchestrationService(
        ISSOUserProcessingService ssoUserProcessingService,
        ITokenProcessingService tokenProcessingService,
        ISessionProcessingService sessionProcessingService)
    {
        this.ssoUserProcessingService = ssoUserProcessingService;
        this.tokenProcessingService = tokenProcessingService;
        this.sessionProcessingService = sessionProcessingService;
    }

    public async ValueTask<Token> LoginAsync(string username, string password)
    {
        SSOUser user = ssoUserProcessingService.FindByUserAndPassword(username, password);

        if (user == null)
            throw new SecurityException("Access Denied!");

        sessionProcessingService.SetUser(user);
        Token token = await tokenProcessingService.AddTokenForUserIdAsync(user.Id);
        return token;
    }

    public async ValueTask Logout(string tokenId = null)
    {
        tokenId ??= sessionProcessingService.GetString("token");
        await tokenProcessingService.DeleteTokenAsync(tokenId);
        sessionProcessingService.Clear();
    }

    public async ValueTask<Token> GenerateForgotPasswordToken(string id)
    {
        string userId = ssoUserProcessingService.FindById(id)?.Id;

        if (userId == null)
            throw new SecurityException("Access Denied!");

        return await tokenProcessingService.GenerateForgottenPasswordToken(userId);
    }
}