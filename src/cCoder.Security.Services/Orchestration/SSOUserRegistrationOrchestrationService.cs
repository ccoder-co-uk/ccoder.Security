using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace cCoder.Security.Services.Orchestration;

public class SSOUserRegistrationOrchestrationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ITokenProcessingService tokenProcessingService) : ISSOUserOrchestrationService
{
    public IQueryable<SSOUser> GetAllSSOUsers() =>
        ssoUserProcessingService.GetAllSSOUsers();

    public async ValueTask<(SSOUser, string)> Register(RegisterUser registerForm)
    {
        ValidateRegisterForm(registerForm);

        SSOUser mappedUser = MapToSSOUser(registerForm);

        SSOUser user = await ssoUserProcessingService.RegisterSSOUserAsync(mappedUser);
        Token confirmationToken = await tokenProcessingService.GenerateConfirmationToken(user.Id);
        return (user, confirmationToken.Id);
    }

    public async ValueTask<SSOUser> UpdateSSOUserAsync(string username, SSOUser item)
    {
        SSOUser user = GetAllSSOUsers()
            .FirstOrDefault(user => user.Id == username);

        if (user == null)
            throw new SecurityException("Access Denied!");

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

        SSOUser mappedUser = MapToSSOUser(registerForm);
        mappedUser.Id = userId;

        Token token = tokenProcessingService.GetInvitationToken(tokenId);

        if (token == null || token.UserName != mappedUser.Id)
            throw new SecurityException("Access Denied!");

        SSOUser user = ssoUserProcessingService.FindById(token.UserName);

        if (user == null)
            throw new SecurityException("Access Denied!");

        user.PasswordHash = registerForm.Password;
        user.LockoutEnabled = false;
        user.DisplayName = registerForm.DisplayName;

        await tokenProcessingService.DeleteTokenAsync(token.Id);

        return await ssoUserProcessingService.UpdateSSOUserAsync(user);
    }

    public async ValueTask ConfirmRegistration(string tokenId)
    {
        Token token = tokenProcessingService.GetConfirmationToken(tokenId);

        if (token == null)
            throw new SecurityException("Access Denied!");

        SSOUser user = ssoUserProcessingService.FindById(token.UserName);

        if (user == null)
            throw new SecurityException("Access Denied!");

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
            throw new SecurityException("Access Denied!");

        SSOUser user = ssoUserProcessingService.FindById(token.UserName);

        if (user == null)
            throw new SecurityException("Access Denied!");

        user.PasswordHash = newPassword;
        await ssoUserProcessingService.UpdateSSOUserAsync(user);
        await tokenProcessingService.DeleteTokenAsync(token.Id);
    }

    public async ValueTask ChangePassword(string username, string oldPassword, string newPassword)
    {
        SSOUser user = ssoUserProcessingService.FindByUserAndPassword(username, oldPassword);

        if (user == null)
            throw new SecurityException("Access Denied!");

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

    private SSOUser MapToSSOUser(RegisterUser registerForm)
        => new()
        {
            Id = registerForm.Email.Split("@")[0],
            DisplayName = registerForm.DisplayName,
            PasswordHash = registerForm.Password,
            Email = registerForm.Email,
            PhoneNumber = registerForm.PhoneNumber
        };
}