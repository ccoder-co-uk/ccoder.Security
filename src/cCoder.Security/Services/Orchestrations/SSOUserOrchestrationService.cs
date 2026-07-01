using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace cCoder.Security.Services.Orchestrations;
internal class SSOUserOrchestrationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ITokenProcessingService tokenProcessingService,
    ISSORoleProcessingService roleProcessingService,
    ISSOUserRoleProcessingService userRoleProcessingService,
    ISSOUserRoleOrchestrationService userRoleOrchestrationService,
    IAccountEventService accountEventService,
    ILogger<SSOUserOrchestrationService> log) : ISSOUserOrchestrationService
{
    public IQueryable<SSOUser> GetAllSSOUsers() =>
        ssoUserProcessingService.GetAllSSOUsers();

    public async ValueTask<(SSOUser, string)> Register(RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm);

        SSOUser mappedUser = MapToSSOUser(registerForm);

        (SSOUser user, bool created) = await RegisterOrReturnExistingUserAsync(registerForm, mappedUser);

        if (!created)
            return (Sanitize(user), null);

        await TryAttachBootstrapTenantRoleAsync(registerForm, user);
        Token confirmationToken = await tokenProcessingService.GenerateConfirmationToken(user.Id);
        await accountEventService.RaiseRegistrationCreatedEventAsync(user, registerForm, confirmationToken.Id);
        return (Sanitize(user), confirmationToken.Id);
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
        ValidateRegisterForm(registerForm, requirePassword: false);

        SSOUser mappedUser = MapToSSOUser(registerForm);

        (SSOUser user, bool created) = await InviteOrReturnExistingUserAsync(registerForm, mappedUser);

        if (!created)
            return (Sanitize(user), null);

        Token inviteToken = await tokenProcessingService.GenerateInvitationToken(user.Id);
        await accountEventService.RaiseInvitationCreatedEventAsync(user, registerForm, inviteToken.Id);
        return (Sanitize(user), inviteToken.Id);
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

        SSOUser updatedUser = await ssoUserProcessingService.UpdateSSOUserAsync(user);
        await accountEventService.RaiseInvitationAcceptedEventAsync(updatedUser, registerForm, tokenId);
        return updatedUser;
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
        await accountEventService.RaiseInvitationCreatedEventAsync(user, null, newToken.Id);

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
        await accountEventService.RaiseRegistrationConfirmedEventAsync(user, tokenId);
    }

    private static void ValidateRegisterForm(RegisterUser registerForm, bool requirePassword = true)
    {
        if (!registerForm.Email.Contains('@'))
            throw new ValidationException("Invalid email provided");

        if (string.IsNullOrEmpty(registerForm.DisplayName))
            throw new ValidationException("Display name cannot be empty");

        if (requirePassword && string.IsNullOrEmpty(registerForm.Password))
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

    private async ValueTask<(SSOUser User, bool Created)> RegisterOrReturnExistingUserAsync(
        RegisterUser registerForm,
        SSOUser mappedUser)
    {
        try
        {
            return (await ssoUserProcessingService.RegisterSSOUserAsync(mappedUser), true);
        }
        catch (ValidationException exception) when (exception.Message == "Email exists")
        {
            return (GetExistingUserByEmail(registerForm.Email), false);
        }
    }

    private async ValueTask<(SSOUser User, bool Created)> InviteOrReturnExistingUserAsync(
        RegisterUser registerForm,
        SSOUser mappedUser)
    {
        try
        {
            return (await ssoUserProcessingService.InviteSSOUserAsync(mappedUser), true);
        }
        catch (ValidationException exception) when (exception.Message == "Email exists")
        {
            return (GetExistingUserByEmail(registerForm.Email), false);
        }
    }

    private SSOUser GetExistingUserByEmail(string email) =>
        ssoUserProcessingService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(user => user.Email == email);

    private static SSOUser Sanitize(SSOUser user)
    {
        if (user is not null)
            user.PasswordHash = null;

        return user;
    }

    private async ValueTask TryAttachBootstrapTenantRoleAsync(RegisterUser registerForm, SSOUser user)
    {
        if (string.IsNullOrWhiteSpace(registerForm.TenantId))
            return;

        if (userRoleProcessingService.GetAllSSOUserRoles().Any())
            return;

        SSORole role = roleProcessingService
            .GetAllSSORoles(ignoreFilters: true)
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


