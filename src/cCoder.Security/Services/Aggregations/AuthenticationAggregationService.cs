// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using System.Security;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class AuthenticationAggregationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ITokenProcessingService tokenProcessingService,
    ISessionProcessingService sessionProcessingService,
    IAccountEventProcessingService accountEventProcessingService,
    ILoggingProcessingService loggingProcessingService)
        : IAuthenticationAggregationService
{
    public ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidateTokenOnIssue(userId: userId, tokenUse: tokenUse);

            return await tokenProcessingService.AddTokenForUserIdAsync(
                userId: userId,
                tokenUse: tokenUse);
        });

    public ValueTask<Token> LoginAsync(string username, string password) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidateCredentialsOnLogin(username: username, password: password);

            return await LoginCoreAsync(
                username: username,
                password: password);
        });

    public ValueTask LogoutAsync() =>
        TryCatch(operation: async () =>
        {
            ValidateAuthenticationOnLogout();

            await LogoutCoreAsync();
        });

    public ValueTask ChangePasswordAsync(
        string username,
        string oldPassword,
        string newPassword) =>
        TryCatch(operation: async () =>
        {
            ValidatePasswordOnChange(
                username: username,
                oldPassword: oldPassword,
                newPassword: newPassword);

            await ChangePasswordCoreAsync(
                username: username,
                oldPassword: oldPassword,
                newPassword: newPassword);
        });

    public ValueTask<Token> ForgotPasswordAsync(string email) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidatePasswordOnForget(email: email);

            return await ForgotPasswordCoreAsync(email: email);
        });

    public ValueTask ConfirmForgotPasswordAsync(
        string tokenId,
        string userId,
        string newPassword,
        string confirmNewPassword) =>
        TryCatch(operation: async () =>
        {
            ValidateForgottenPasswordOnConfirm(
                tokenId: tokenId,
                userId: userId,
                newPassword: newPassword,
                confirmNewPassword: confirmNewPassword);

            await ConfirmForgotPasswordCoreAsync(
                tokenId: tokenId,
                userId: userId,
                newPassword: newPassword,
                confirmNewPassword: confirmNewPassword);
        });

    private async ValueTask<Token> LoginCoreAsync(
        string username,
        string password)
    {
        SSOUser user = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username: username, password: password);

        if (user == null)
        { throw new SecurityException("Access Denied!"); }

        sessionProcessingService.SetSSOUser(user: user);

        Token token = await tokenProcessingService
            .AddTokenForUserIdAsync(userId: user.Id, tokenUse: TokenUse.Auth);

        return token;
    }

    private async ValueTask LogoutCoreAsync()
    {
        string tokenId = sessionProcessingService.GetString(key: "token");
        await tokenProcessingService.DeleteTokenAsync(tokenId: tokenId);
        sessionProcessingService.Clear();
    }

    private async ValueTask ChangePasswordCoreAsync(
        string username,
        string oldPassword,
        string newPassword)
    {
        SSOUser user = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username: username, password: oldPassword);

        if (user == null)
        {
            loggingProcessingService.LogWarning(
                message: $"User not found: {username}");

            throw new SecurityException("Access Denied!");
        }

        ssoUserProcessingService.ValidatePassword(password: newPassword);

        user.PasswordHash = newPassword;

        await ssoUserProcessingService.UpdateSSOUserAsync(item: user);
    }

    private async ValueTask<Token> ForgotPasswordCoreAsync(string email)
    {
        SSOUser user = ssoUserProcessingService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: user => user.Email == email);

        if (user == null)
        { throw new SecurityException("User not found"); }

        Token token = await tokenProcessingService.GenerateForgottenPasswordToken(userId: user.Id);

        SecurityAccountEventRequest accountEventRequest = new()
        {
            Kind = SecurityAccountEventKind.PasswordResetRequested,
            User = user,
            RegisterForm = null,
            Token = token.Id
        };

        await accountEventProcessingService.RaiseSecurityAccountEventRequestAsync(
            accountEventRequest: accountEventRequest);

        return token;
    }

    private async ValueTask ConfirmForgotPasswordCoreAsync(
        string tokenId,
        string userId,
        string newPassword,
        string confirmNewPassword)
    {
        if (!newPassword.Equals(value: confirmNewPassword))
        { throw new SecurityException("Passwords do not match"); }

        Token token = tokenProcessingService.GetForgottenPasswordToken(tokenId: tokenId);

        if (token == null || token.UserName != userId)
        {
            loggingProcessingService.LogWarning(
                message: token == null
                    ? "Token not found"
                    : $"Token username does not match given user ID: {token.UserName} / {userId}");

            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(ssoUserId: token.UserName);

        if (user == null)
        {
            loggingProcessingService.LogWarning(
                message: $"Token user not found: {token.UserName}");

            throw new SecurityException("Access Denied!");
        }

        user.PasswordHash = newPassword;
        user.LockoutEnabled = false;
        user.AccessFailedCount = 0;

        await ssoUserProcessingService.UpdateSSOUserAsync(item: user);
        await tokenProcessingService.DeleteTokenAsync(tokenId: token.Id);
    }

}