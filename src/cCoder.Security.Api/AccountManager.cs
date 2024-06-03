using cCoder.Security.Api.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace cCoder.Security.Api;

public class AccountManager(
    IAuthenticationOrchestrationService authService,
    ISSOUserOrchestrationService registrationService,
    ISSOUserProcessingService userService) : IAccountManager
{
    public async ValueTask ChangePasswordAsync(string username, string oldPassword, string newPassword) =>
        await registrationService.ChangePassword(username, oldPassword, newPassword);

    public async ValueTask ConfirmForgotPasswordAsync(string token, string userId, string newPassword, string confirmNewPassword) =>
        await registrationService.ConfirmForgotPassword(token, userId, newPassword, confirmNewPassword);
    
    public async ValueTask ConfirmRegistrationAsync(string token) =>
        await registrationService.ConfirmRegistration(token);

    public async ValueTask<Token> ForgotPasswordAsync(string email)
    {
        SSOUser user = registrationService
            .GetAllSSOUsers()
            .IgnoreQueryFilters()
            .FirstOrDefault(user => user.Email == email);

        if (user == null)
            throw new SecurityException("User not found");

        return await authService.GenerateForgotPasswordToken(user.Id);
    }

    public async ValueTask<Token> LoginAsync(string username, string password) =>
        await authService.LoginAsync(username, password);

    public async ValueTask LogoutAsync() =>
        await authService.Logout();

    public SSOUser Me() =>
        userService.Me();

    public async ValueTask<(SSOUser, string)> RegisterAsync(RegisterUser registerForm)
    {
        try
        {
            (SSOUser, string) result = await registrationService.Register(registerForm);
            result.Item1.PasswordHash = null;
            return result;
        }
        catch (ValidationException ex)
        {
            if (ex.Message == "Email exists")
            {
                SSOUser user = userService.GetAllSSOUsers(ignoreFilters: true)
                    .Where(u => u.Email == registerForm.Email)
                    .FirstOrDefault();

                if (user is not null)
                    user.PasswordHash = null;

                return (user, null);
            }
            else
                throw;
        }
    }

    public async ValueTask<(SSOUser, string)> InviteUserAsync(RegisterUser registerForm)
    {
        try
        {
            (SSOUser, string) result = await registrationService.InviteUserAsync(registerForm);
            result.Item1.PasswordHash = null;
            return result;
        }
        catch (ValidationException ex)
        {
            if (ex.Message == "Email exists")
            {
                SSOUser user = userService.GetAllSSOUsers(ignoreFilters: true)
                    .Where(u => u.Email == registerForm.Email)
                    .FirstOrDefault();

                if (user is not null)
                    user.PasswordHash = null;

                return (user, null);
            }
            else
                throw;
        }
    }

    public async ValueTask<SSOUser> AcceptInviteAsync(RegisterUser user, string userId, string inviteToken) =>
        await registrationService.AcceptInviteAsync(user, userId, inviteToken);
}