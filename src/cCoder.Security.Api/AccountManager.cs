using System.ComponentModel.DataAnnotations;
using System.Security;
using Microsoft.EntityFrameworkCore;
using cCoder.Security.Api.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Api
{
    public class AccountManager : IAccountManager
    {
        private readonly IAuthenticationOrchestrationService authService;
        private readonly ISSOUserOrchestrationService registrationService;
        private readonly ISSOUserProcessingService userService;

        public AccountManager(
            IAuthenticationOrchestrationService authService,
            ISSOUserOrchestrationService registrationService,
            ISSOUserProcessingService userService)
        {
            this.authService = authService;
            this.registrationService = registrationService;
            this.userService = userService;
        }

        public async ValueTask ChangePasswordAsync(string username, string oldPassword, string newPassword) =>
            await registrationService.ChangePassword(username, oldPassword, newPassword);

        public async ValueTask ConfirmForgotPasswordAsync(string token, string userId, string newPassword, string confirmNewPassword) =>
            await registrationService.ConfirmForgotPassword(token, userId, newPassword, confirmNewPassword);
        
        public async ValueTask ConfirmRegistrationAsync(string token) =>
            await registrationService.ConfirmRegistration(token);

        public async ValueTask<Token> ForgotPasswordAsync(string email)
        {
            var user = registrationService
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
                var result = await registrationService.Register(registerForm);
                result.Item1.PasswordHash = null;
                return result;
            }
            catch (ValidationException ex)
            {
                if (ex.Message == "Email exists")
                {
                    var user = registrationService.GetAllSSOUsers()
                        .Where(u => u.Email == registerForm.Email)
                        .FirstOrDefault();
                    user.PasswordHash = null;

                    return (user, null);
                }
                else
                    throw;
            }
        }
    }
}