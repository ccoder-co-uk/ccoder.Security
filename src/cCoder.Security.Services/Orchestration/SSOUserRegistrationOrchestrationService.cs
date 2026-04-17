using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace cCoder.Security.Services.Orchestration;

public class SSOUserRegistrationOrchestrationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ITokenProcessingService tokenProcessingService,
    ISSORoleProcessingService roleProcessingService,
    ISSOUserRoleProcessingService userRoleProcessingService,
    ISSOUserRoleOrchestrationService userRoleOrchestrationService,
    ILogger<SSOUserRegistrationOrchestrationService> log) : ISSOUserOrchestrationService
{
    public IQueryable<SSOUser> GetAllSSOUsers() =>
        ssoUserProcessingService.GetAllSSOUsers();

    public async ValueTask<(SSOUser, string)> Register(RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm);

        SSOUser mappedUser = MapToSSOUser(registerForm);

        SSOUser user = await ssoUserProcessingService.RegisterSSOUserAsync(mappedUser);
        await TryAttachBootstrapTenantRoleAsync(registerForm, user);
        Token confirmationToken = await tokenProcessingService.GenerateConfirmationToken(user.Id);
        return (user, confirmationToken.Id);
    }

    public async ValueTask<SSOUser> UpdateSSOUserAsync(string username, SSOUser item)
    {
        SSOUser user = GetAllSSOUsers()
            .FirstOrDefault(user => user.Id == username);

        if (user == null)
        {
            log.LogWarning($"User not found: {username}");
            throw new SecurityException("Access Denied!");
        }

        user.DisplayName = item.DisplayName;
        user.PhoneNumber = item.PhoneNumber;
        user.Email = item.Email;

        return await ssoUserProcessingService.UpdateSSOUserAsync(user);
    }

    public ValueTask DeleteSSOUserAsync(SSOUser item) =>
        ssoUserProcessingService.DeleteSSOUserAsync(item);

    public async ValueTask<(SSOUser, string)> InviteUserAsync(RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm);

        SSOUser mappedUser = MapToSSOUser(registerForm);

        SSOUser user = await ssoUserProcessingService.InviteSSOUserAsync(mappedUser);
        Token inviteToken = await tokenProcessingService.GenerateInvitationToken(user.Id);
        return (user, inviteToken.Id);
    }

    public async ValueTask<SSOUser> AcceptInviteAsync(RegisterUser registerForm, string userId, string tokenId)
    {
        ValidateRegisterForm(registerForm);

        Token token = tokenProcessingService.GetInvitationToken(tokenId);

        if (token == null || token.UserName != userId)
        {
            log.LogWarning(token == null ? "Token not found" : $"Token username does not match given user ID: {token.UserName} / {userId}");
            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(token.UserName);

        if (user == null)
        {
            log.LogWarning($"Token user not found: {token.UserName}");
            throw new SecurityException("Access Denied!");
        }

        user.PasswordHash = registerForm.Password;
        user.LockoutEnabled = false;
        user.DisplayName = registerForm.DisplayName;

        await tokenProcessingService.DeleteTokenAsync(token.Id);

        return await ssoUserProcessingService.UpdateSSOUserAsync(user);
    }

    public async ValueTask<string> RegenerateUserInviteToken(string userId)
    {
        SSOUser user = ssoUserProcessingService
            .FindById(userId);

        if (user == null)
        {
            log.LogWarning($"User not found: {userId}");
            throw new SecurityException("Access Denied!");
        }

        var newToken = await tokenProcessingService.GenerateInvitationToken(userId);

        return newToken.Id;
    }

    public async ValueTask ConfirmRegistration(string tokenId)
    {
        Token token = tokenProcessingService.GetConfirmationToken(tokenId);

        if (token == null)
        {
            log.LogWarning($"Token not found");
            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(token.UserName);

        if (user == null)
        {
            log.LogWarning($"Token user not found: {token.UserName}");
            throw new SecurityException("Access Denied!");
        }

        user.EmailConfirmed = true;
        await ssoUserProcessingService.UpdateSSOUserAsync(user);
        await tokenProcessingService.DeleteTokenAsync(token.Id);
    }

    public async ValueTask ConfirmForgotPassword(string tokenId, string userId, string newPassword, string confirmNewPassword)
    {
        if (!newPassword.Equals(confirmNewPassword))
            throw new SecurityException("Passwords do not match");

        Token token = tokenProcessingService.GetForgottenPasswordToken(tokenId);

        if (token == null || token.UserName != userId)
        {
            log.LogWarning(token == null ? "Token not found" : $"Token username does not match given user ID: {token.UserName} / {userId}");
            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(token.UserName);

        if (user == null)
        {
            log.LogWarning($"Token user not found: {token.UserName}");
            throw new SecurityException("Access Denied!");
        }

        user.PasswordHash = newPassword;
        user.LockoutEnabled = false;
        user.AccessFailedCount = 0;

        await ssoUserProcessingService.UpdateSSOUserAsync(user);
        await tokenProcessingService.DeleteTokenAsync(token.Id);
    }

    public async ValueTask ChangePassword(string username, string oldPassword, string newPassword)
    {
        SSOUser user = await ssoUserProcessingService
            .FindByUserAndPasswordAsync(username, oldPassword);

        if (user == null)
        {
            log.LogWarning($"User not found: {username}");
            throw new SecurityException("Access Denied!");
        }

        ssoUserProcessingService.ValidatePassword(newPassword);

        user.PasswordHash = newPassword;

        await ssoUserProcessingService.UpdateSSOUserAsync(user);
    }

    private static void ValidateRegisterForm(RegisterUser registerForm)
    {
        if (!registerForm.Email.Contains('@'))
            throw new ValidationException("Invalid email provided");

        if (string.IsNullOrEmpty(registerForm.DisplayName))
            throw new ValidationException("Display name cannot be empty");

        if (string.IsNullOrEmpty(registerForm.Password))
            throw new ValidationException("Password cannot be empty");
    }

    private static SSOUser MapToSSOUser(RegisterUser registerForm) => new()
    {
        Id = registerForm.Email.Split("@")[0],
        DisplayName = registerForm.DisplayName,
        PasswordHash = registerForm.Password,
        Email = registerForm.Email,
        PhoneNumber = registerForm.PhoneNumber
    };

    private async ValueTask TryAttachBootstrapTenantRoleAsync(RegisterUser registerForm, SSOUser user)
    {
        if (string.IsNullOrWhiteSpace(registerForm.TenantId))
            return;

        if (userRoleProcessingService.GetAllSSOUserRoles().Any())
            return;

        SSORole role = roleProcessingService
            .GetAllSSORoles()
            .FirstOrDefault(foundRole =>
                foundRole.TenantId == registerForm.TenantId
                && foundRole.UsersArePortalAdmins);

        if (role is null)
            throw new ValidationException(
                $"Bootstrap administrator role not found for tenant '{registerForm.TenantId}'.");

        await userRoleOrchestrationService.AddSSOUserRoleAsync(new SSOUserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });
    }
}
