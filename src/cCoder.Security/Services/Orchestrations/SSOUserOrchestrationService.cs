// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

    public async ValueTask<(SSOUser, string)> RegisterUserAsync(RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm: registerForm);

        SSOUser mappedUser = MapToSSOUser(registerForm: registerForm);

        (SSOUser user, bool created) = await RegisterOrReturnExistingUserAsync(registerForm: registerForm, mappedUser: mappedUser);

        if (!created)
        { return (Sanitize(user: user), null); }

        await TryAttachBootstrapTenantRoleAsync(registerForm: registerForm, user: user);
        Token confirmationToken = await tokenProcessingService.GenerateConfirmationToken(userId: user.Id);

        await accountEventService.RaiseRegistrationCreatedSSOUserRegisterUserEventAsync(
            user: user,
            registerForm: registerForm,
            token: confirmationToken.Id);

        return (Sanitize(user: user), confirmationToken.Id);
    }

    public async ValueTask<SSOUser> UpdateSSOUserAsync(string username, SSOUser updatedSSOUser)
    {
        SSOUser user = GetAllSSOUsers()
            .FirstOrDefault(predicate: user => user.Id == username);

        if (user == null)
        {
            log.LogWarning(message: $"User not found: {username}");
            throw new SecurityException("Access Denied!");
        }

        user.DisplayName = updatedSSOUser.DisplayName;
        user.PhoneNumber = updatedSSOUser.PhoneNumber;
        user.Email = updatedSSOUser.Email;

        return await ssoUserProcessingService.UpdateSSOUserAsync(item: user);
    }

    public ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser) =>
        ssoUserProcessingService.DeleteSSOUserAsync(item: deletedSSOUser);

    public async ValueTask<(SSOUser, string)> InviteRegisterUserAsync(RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm: registerForm, requirePassword: false);

        SSOUser mappedUser = MapToSSOUser(registerForm: registerForm);

        (SSOUser user, bool created) = await InviteOrReturnExistingUserAsync(registerForm: registerForm, mappedUser: mappedUser);

        if (!created)
        { return (Sanitize(user: user), null); }

        Token inviteToken = await tokenProcessingService.GenerateInvitationToken(userId: user.Id);

        await accountEventService.RaiseInvitationCreatedSSOUserRegisterUserEventAsync(
            user: user,
            registerForm: registerForm,
            token: inviteToken.Id);

        return (Sanitize(user: user), inviteToken.Id);
    }

    public async ValueTask<SSOUser> AcceptRegisterUserInviteAsync(
        RegisterUser registerForm,
        string userId,
        string tokenId)
    {
        ValidateRegisterForm(registerForm: registerForm);

        Token token = tokenProcessingService.GetInvitationToken(tokenId: tokenId);

        if (token == null || token.UserName != userId)
        {
            log.LogWarning(message: token == null ? "Token not found" : $"Token username does not match given user ID: {token.UserName} / {userId}");
            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(ssoUserId: token.UserName);

        if (user == null)
        {
            log.LogWarning(message: $"Token user not found: {token.UserName}");
            throw new SecurityException("Access Denied!");
        }

        user.PasswordHash = registerForm.Password;
        user.LockoutEnabled = false;
        user.DisplayName = registerForm.DisplayName;

        await tokenProcessingService.DeleteTokenAsync(tokenId: token.Id);

        SSOUser updatedUser = await ssoUserProcessingService.UpdateSSOUserAsync(item: user);

        await accountEventService.RaiseInvitationAcceptedSSOUserRegisterUserEventAsync(
            user: updatedUser,
            registerForm: registerForm,
            token: tokenId);

        return updatedUser;
    }

    public async ValueTask<string> RegenerateUserInviteToken(string userId)
    {
        SSOUser user = ssoUserProcessingService
            .FindById(ssoUserId: userId);

        if (user == null)
        {
            log.LogWarning(message: $"User not found: {userId}");
            throw new SecurityException("Access Denied!");
        }

        var newToken = await tokenProcessingService.GenerateInvitationToken(userId: userId);

        await accountEventService.RaiseInvitationCreatedSSOUserRegisterUserEventAsync(
            user: user,
            registerForm: null,
            token: newToken.Id);

        return newToken.Id;
    }

    public async ValueTask ConfirmRegistration(string tokenId)
    {
        Token token = tokenProcessingService.GetConfirmationToken(tokenId: tokenId);

        if (token == null)
        {
            log.LogWarning(message: $"Token not found");
            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(ssoUserId: token.UserName);

        if (user == null)
        {
            log.LogWarning(message: $"Token user not found: {token.UserName}");
            throw new SecurityException("Access Denied!");
        }

        user.EmailConfirmed = true;
        await ssoUserProcessingService.UpdateSSOUserAsync(item: user);
        await tokenProcessingService.DeleteTokenAsync(tokenId: token.Id);

        await accountEventService.RaiseRegistrationConfirmedSSOUserEventAsync(
            user: user,
            token: tokenId);
    }

    private static void ValidateRegisterForm(RegisterUser registerForm, bool requirePassword = true)
    {
        if (!registerForm.Email.Contains(value: '@'))
        { throw new ValidationException("Invalid email provided"); }

        if (string.IsNullOrEmpty(value: registerForm.DisplayName))
        { throw new ValidationException("Display name cannot be empty"); }

        if (requirePassword && string.IsNullOrEmpty(value: registerForm.Password))
        { throw new ValidationException("Password cannot be empty"); }
    }

    private static SSOUser MapToSSOUser(RegisterUser registerForm) =>
        new()
        {
            Id = registerForm.Email.Split(separator: "@")[0],
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
            return (await ssoUserProcessingService.RegisterSSOUserAsync(item: mappedUser), true);
        }
        catch (ValidationException exception) when (exception.Message == "Email exists")
        {
            return (GetExistingUserByEmail(email: registerForm.Email), false);
        }
    }

    private async ValueTask<(SSOUser User, bool Created)> InviteOrReturnExistingUserAsync(
        RegisterUser registerForm,
        SSOUser mappedUser)
    {
        try
        {
            return (await ssoUserProcessingService.InviteSSOUserAsync(user: mappedUser), true);
        }
        catch (ValidationException exception) when (exception.Message == "Email exists")
        {
            return (GetExistingUserByEmail(email: registerForm.Email), false);
        }
    }

    private SSOUser GetExistingUserByEmail(string email) =>
        ssoUserProcessingService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: user => user.Email == email);

    private static SSOUser Sanitize(SSOUser user)
    {
        if (user is not null)
        { user.PasswordHash = null; }

        return user;
    }

    private async ValueTask TryAttachBootstrapTenantRoleAsync(RegisterUser registerForm, SSOUser user)
    {
        if (string.IsNullOrWhiteSpace(value: registerForm.TenantId))
        { return; }

        if (userRoleProcessingService
            .GetAllSSOUserRoles()
            .Any())
        { return; }

        SSORole role = roleProcessingService
            .GetAllSSORoles(ignoreFilters: true)
            .FirstOrDefault(predicate: foundRole =>
                foundRole.TenantId == registerForm.TenantId
                && foundRole.UsersArePortalAdmins);

        if (role is null)
        {
            throw new ValidationException(
            $"Bootstrap administrator role not found for tenant '{registerForm.TenantId}'.");
        }

        await userRoleOrchestrationService.AddSSOUserRoleAsync(userRole: new SSOUserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });
    }
}