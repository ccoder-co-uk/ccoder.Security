using Security.Objects.DTOs;
using Security.Objects.Entities;
using System.Threading.Tasks;

namespace Security.Api.Interfaces
{
    public interface IAccountManager
    {
        SSOUser Me();
        ValueTask<Token> LoginAsync(string username, string password);
        ValueTask LogoutAsync();
        ValueTask<(SSOUser, string)> RegisterAsync(RegisterUser registerForm);
        ValueTask ChangePasswordAsync(string username, string oldPassword, string newPassword);
        ValueTask<Token> ForgotPasswordAsync(string email);
        ValueTask ConfirmRegistrationAsync(string token);
        ValueTask ConfirmForgotPasswordAsync(string token, string userId, string newPassword, string confirmNewPassword);
    }
}