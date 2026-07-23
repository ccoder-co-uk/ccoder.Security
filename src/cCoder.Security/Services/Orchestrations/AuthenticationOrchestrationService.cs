// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using System.Security;

namespace cCoder.Security.Services.Orchestrations;

internal class AuthenticationOrchestrationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ITokenProcessingService tokenProcessingService,
    ISessionProcessingService sessionProcessingService,
    IAccountEventService accountEventService,
    ILogger<AuthenticationOrchestrationService> log)
    : IAuthenticationOrchestrationService
{
    public SSOUser Me() =>
        Sanitize(user: ssoUserProcessingService.Me());

    public async ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse) =>
        await tokenProcessingService.AddTokenForUserIdAsync(userId: userId, tokenUse: tokenUse);

    public async ValueTask<Token> LoginAsync(string username, string password)
    {
        SSOUser user = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username: username, password: password);

        if (user == null)
            throw new SecurityException("Access Denied!");

        sessionProcessingService.SetUser(user: user);

        Token token = await tokenProcessingService
            .AddTokenForUserIdAsync(userId: user.Id, tokenUse: TokenUse.Auth);

        return token;
    }

    public async ValueTask LogoutAsync()
    {
        string tokenId = sessionProcessingService.GetString(key: "token");
        await tokenProcessingService.DeleteTokenAsync(tokenId: tokenId);
        sessionProcessingService.Clear();
    }

    public async ValueTask ChangePasswordAsync(string username, string oldPassword, string newPassword)
    {
        SSOUser user = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username: username, password: oldPassword);

        if (user == null)
        {
            log.LogWarning(message: $"User not found: {username}");
            throw new SecurityException("Access Denied!");
        }

        ssoUserProcessingService.ValidatePassword(password: newPassword);

        user.PasswordHash = newPassword;

        await ssoUserProcessingService.UpdateSSOUserAsync(item: user);
    }

    public async ValueTask<Token> ForgotPasswordAsync(string email)
    {
        SSOUser user = ssoUserProcessingService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: user => user.Email == email);

        if (user == null)
            throw new SecurityException("User not found");

        Token token = await tokenProcessingService.GenerateForgottenPasswordToken(userId: user.Id);
        await accountEventService.RaisePasswordResetRequestedEventAsync(user: user, token: token.Id);
        return token;
    }

    public async ValueTask ConfirmForgotPasswordAsync(
        string tokenId,
        string userId,
        string newPassword,
        string confirmNewPassword)
    {
        if (!newPassword.Equals(value: confirmNewPassword))
            throw new SecurityException("Passwords do not match");

        Token token = tokenProcessingService.GetForgottenPasswordToken(tokenId: tokenId);

        if (token == null || token.UserName != userId)
        {
            log.LogWarning(message: token == null ? "Token not found" : $"Token username does not match given user ID: {token.UserName} / {userId}");
            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(id: token.UserName);

        if (user == null)
        {
            log.LogWarning(message: $"Token user not found: {token.UserName}");
            throw new SecurityException("Access Denied!");
        }

        user.PasswordHash = newPassword;
        user.LockoutEnabled = false;
        user.AccessFailedCount = 0;

        await ssoUserProcessingService.UpdateSSOUserAsync(item: user);
        await tokenProcessingService.DeleteTokenAsync(tokenId: token.Id);
    }

    private static SSOUser Sanitize(SSOUser user) =>
        user is null
            ? null
            : new SSOUser
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed
            };
}