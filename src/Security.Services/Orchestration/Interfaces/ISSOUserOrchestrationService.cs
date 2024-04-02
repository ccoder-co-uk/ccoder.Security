using Security.Objects.DTOs;
using Security.Objects.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Security.Services.Orchestration.Interfaces
{
    public interface ISSOUserOrchestrationService
    {
        ValueTask<(SSOUser, string)> Register(RegisterUser registerForm);
        ValueTask ConfirmForgotPassword(string token, string userId, string newPassword, string confirmNewPassword);
        ValueTask ConfirmRegistration(string tokenId);
        ValueTask ChangePassword(string username, string oldPassword, string newPassword);

        ValueTask<SSOUser> UpdateSSOUserAsync(string username, SSOUser item);
        ValueTask DeleteSSOUserAsync(SSOUser item);
        IQueryable<SSOUser> GetAllSSOUsers();
    }
}