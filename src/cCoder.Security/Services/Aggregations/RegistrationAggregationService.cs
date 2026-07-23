// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class RegistrationAggregationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ITenantProcessingService tenantProcessingService,
    ITokenProcessingService tokenProcessingService,
    ISSORoleProcessingService roleProcessingService,
    ISSOUserRoleProcessingService userRoleProcessingService,
    IAccountEventProcessingService accountEventProcessingService,
    ILoggingProcessingService loggingProcessingService)
        : IRegistrationAggregationService
{
    public ValueTask<RegisterUser> RegisterUserAsync(
        RegisterUser registerForm) =>
        TryCatch<RegisterUser>(operation: async () =>
        {
            ValidateRegistrationOnRegister(registerForm: registerForm);

            return await RegisterUserCoreAsync(registerForm: registerForm);
        });

    public ValueTask<RegisterUser> InviteRegisterUserAsync(
        RegisterUser registerForm) =>
        TryCatch<RegisterUser>(operation: async () =>
        {
            ValidateRegistrationOnInvite(registerForm: registerForm);

            return await InviteRegisterUserCoreAsync(
                registerForm: registerForm);
        });

    public ValueTask<RegisterUser> AcceptRegisterUserInviteAsync(
        RegisterUser registerForm,
        string userId,
        string tokenId) =>
        TryCatch<RegisterUser>(operation: async () =>
        {
            ValidateRegistrationOnAccept(
                registerForm: registerForm,
                userId: userId,
                tokenId: tokenId);

            return await AcceptRegisterUserInviteCoreAsync(
                registerForm: registerForm,
                userId: userId,
                tokenId: tokenId);
        });

    public ValueTask<string> RegenerateUserInviteToken(string userId) =>
        TryCatch<string>(operation: async () =>
        {
            ValidateInvitationTokenOnRegenerate(userId: userId);

            return await RegenerateUserInviteTokenCoreAsync(userId: userId);
        });

    public ValueTask ConfirmRegistration(string tokenId) =>
        TryCatch(operation: async () =>
        {
            ValidateRegistrationOnConfirm(tokenId: tokenId);

            await ConfirmRegistrationCoreAsync(tokenId: tokenId);
        });

    public ValueTask SetupRegisterUserAsync(RegisterUser newRegisterUser) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: newRegisterUser);
            NormalizeRegisterUser(registerUser: newRegisterUser);

            await AddBootstrapTenantAsync(
                newRegisterUser: newRegisterUser);

            RegisterUser registration =
                await RegisterUserCoreAsync(
                    registerForm: newRegisterUser);

            await ConfirmRegistrationCoreAsync(
                tokenId: registration.Token);
        });

    private async ValueTask<RegisterUser> RegisterUserCoreAsync(
        RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm: registerForm);

        SSOUser mappedUser = MapToSSOUser(registerForm: registerForm);

        (SSOUser user, bool created) = await RegisterOrReturnExistingUserAsync(registerForm: registerForm, mappedUser: mappedUser);

        if (!created)
        {
            registerForm.User = Sanitize(user: user);
            registerForm.Token = null;

            return registerForm;
        }

        await TryAttachBootstrapTenantRoleAsync(registerForm: registerForm, user: user);
        Token confirmationToken = await tokenProcessingService.GenerateConfirmationToken(userId: user.Id);

        SecurityAccountEventRequest accountEventRequest = new()
        {
            Kind = SecurityAccountEventKind.RegistrationCreated,
            User = user,
            RegisterForm = registerForm,
            Token = confirmationToken.Id
        };

        await accountEventProcessingService.RaiseSecurityAccountEventRequestAsync(
            accountEventRequest: accountEventRequest);

        registerForm.User = Sanitize(user: user);
        registerForm.Token = confirmationToken.Id;

        return registerForm;
    }

    private async ValueTask<RegisterUser> InviteRegisterUserCoreAsync(
        RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm: registerForm, requirePassword: false);

        SSOUser mappedUser = MapToSSOUser(registerForm: registerForm);

        (SSOUser user, bool created) = await InviteOrReturnExistingUserAsync(registerForm: registerForm, mappedUser: mappedUser);

        if (!created)
        {
            registerForm.User = Sanitize(user: user);
            registerForm.Token = null;

            return registerForm;
        }

        Token inviteToken = await tokenProcessingService.GenerateInvitationToken(userId: user.Id);

        SecurityAccountEventRequest accountEventRequest = new()
        {
            Kind = SecurityAccountEventKind.InvitationCreated,
            User = user,
            RegisterForm = registerForm,
            Token = inviteToken.Id
        };

        await accountEventProcessingService.RaiseSecurityAccountEventRequestAsync(
            accountEventRequest: accountEventRequest);

        registerForm.User = Sanitize(user: user);
        registerForm.Token = inviteToken.Id;

        return registerForm;
    }

    private async ValueTask<RegisterUser> AcceptRegisterUserInviteCoreAsync(
        RegisterUser registerForm,
        string userId,
        string tokenId)
    {
        ValidateRegisterForm(registerForm: registerForm);

        Token token = tokenProcessingService.GetInvitationToken(tokenId: tokenId);

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

        user.PasswordHash = registerForm.Password;
        user.LockoutEnabled = false;
        user.DisplayName = registerForm.DisplayName;

        await tokenProcessingService.DeleteTokenAsync(tokenId: token.Id);

        SSOUser updatedUser = await ssoUserProcessingService.UpdateSSOUserAsync(item: user);

        SecurityAccountEventRequest accountEventRequest = new()
        {
            Kind = SecurityAccountEventKind.InvitationAccepted,
            User = updatedUser,
            RegisterForm = registerForm,
            Token = tokenId
        };

        await accountEventProcessingService.RaiseSecurityAccountEventRequestAsync(
            accountEventRequest: accountEventRequest);

        registerForm.User = updatedUser;
        registerForm.Token = tokenId;

        return registerForm;
    }

    private async ValueTask<string> RegenerateUserInviteTokenCoreAsync(
        string userId)
    {
        SSOUser user = ssoUserProcessingService
            .FindById(ssoUserId: userId);

        if (user == null)
        {
            loggingProcessingService.LogWarning(
                message: $"User not found: {userId}");

            throw new SecurityException("Access Denied!");
        }

        var newToken = await tokenProcessingService.GenerateInvitationToken(userId: userId);

        SecurityAccountEventRequest accountEventRequest = new()
        {
            Kind = SecurityAccountEventKind.InvitationCreated,
            User = user,
            RegisterForm = null,
            Token = newToken.Id
        };

        await accountEventProcessingService.RaiseSecurityAccountEventRequestAsync(
            accountEventRequest: accountEventRequest);

        return newToken.Id;
    }

    private async ValueTask ConfirmRegistrationCoreAsync(string tokenId)
    {
        Token token = tokenProcessingService.GetConfirmationToken(tokenId: tokenId);

        if (token == null)
        {
            loggingProcessingService.LogWarning(message: "Token not found");
            throw new SecurityException("Access Denied!");
        }

        SSOUser user = ssoUserProcessingService.FindById(ssoUserId: token.UserName);

        if (user == null)
        {
            loggingProcessingService.LogWarning(
                message: $"Token user not found: {token.UserName}");

            throw new SecurityException("Access Denied!");
        }

        user.EmailConfirmed = true;
        await ssoUserProcessingService.UpdateSSOUserAsync(item: user);
        await tokenProcessingService.DeleteTokenAsync(tokenId: token.Id);

        SecurityAccountEventRequest accountEventRequest = new()
        {
            Kind = SecurityAccountEventKind.RegistrationConfirmed,
            User = user,
            RegisterForm = null,
            Token = tokenId
        };

        await accountEventProcessingService.RaiseSecurityAccountEventRequestAsync(
            accountEventRequest: accountEventRequest);
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

        await userRoleProcessingService.AddSSOUserRoleAsync(item: new SSOUserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });
    }
}