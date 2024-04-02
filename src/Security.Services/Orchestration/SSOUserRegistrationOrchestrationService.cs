using System;
using Security.Objects.DTOs;
using Security.Objects.Entities;
using Security.Services.Orchestration.Interfaces;
using Security.Services.Processing.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Security.Services.Orchestration
{
    public class SSOUserRegistrationOrchestrationService : ISSOUserOrchestrationService
    {
        readonly ISSOUserProcessingService ssoUserProcessingService;
        readonly ITokenProcessingService tokenProcessingService;

        public SSOUserRegistrationOrchestrationService(
            ISSOUserProcessingService ssoUserProcessingService,
            ITokenProcessingService tokenProcessingService)
        {
            this.ssoUserProcessingService = ssoUserProcessingService;
            this.tokenProcessingService = tokenProcessingService;
        }

        public async ValueTask<(SSOUser, string)> Register(RegisterUser registerForm)
        {
            ValidateRegisterForm(registerForm);

            var mappedUser = MapToSSOUser(registerForm);

            var user = await ssoUserProcessingService.RegisterSSOUserAsync(mappedUser);
            var confirmationToken = await tokenProcessingService.GenerateConfirmationToken(user.Id);
            return (user, confirmationToken.Id);
        }

        static void ValidateRegisterForm(RegisterUser registerForm)
        {
            if (!registerForm.Email.Contains('@'))
                throw new ValidationException("Invalid email provided");

            if (string.IsNullOrEmpty(registerForm.DisplayName))
                throw new ValidationException("Display name cannot be empty");

            if (string.IsNullOrEmpty(registerForm.Password))
                throw new ValidationException("Password cannot be empty");
        }

        private SSOUser MapToSSOUser(RegisterUser registerForm)
            => new SSOUser
            {
                Id = registerForm.Email.Split("@")[0],
                DisplayName = registerForm.DisplayName,
                PasswordHash = registerForm.Password,
                Email = registerForm.Email,
                PhoneNumber = registerForm.PhoneNumber
            };

        public async ValueTask ConfirmRegistration(string tokenId)
        {
            var token = tokenProcessingService.GetConfirmationToken(tokenId);

            if (token == null)
                throw new SecurityException("Access Denied!");

            var user = ssoUserProcessingService.FindById(token.UserName);

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

            var token = tokenProcessingService.GetForgottenPasswordToken(tokenId);

            if (token == null || token.UserName != userId)
                throw new SecurityException("Access Denied!");

            var user = ssoUserProcessingService.FindById(token.UserName);

            if (user == null)
                throw new SecurityException("Access Denied!");

            user.PasswordHash = newPassword;
            await ssoUserProcessingService.UpdateSSOUserAsync(user);
            await tokenProcessingService.DeleteTokenAsync(token.Id);
        }

        public async ValueTask ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = ssoUserProcessingService.FindByUserAndPassword(username, oldPassword);

            if (user == null)
                throw new SecurityException("Access Denied!");

            ssoUserProcessingService.ValidatePassword(newPassword);

            user.PasswordHash = newPassword;

            await ssoUserProcessingService.UpdateSSOUserAsync(user);
        }

        public async ValueTask<SSOUser> UpdateSSOUserAsync(string username, SSOUser item)
        {
            var user = GetAllSSOUsers()
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

        public IQueryable<SSOUser> GetAllSSOUsers() =>
            ssoUserProcessingService.GetAllSSOUsers();
    }
}