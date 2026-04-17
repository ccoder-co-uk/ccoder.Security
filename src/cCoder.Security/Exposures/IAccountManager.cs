using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Exposures;

public interface IAccountManager
{
    SSOUser Me();

    ValueTask<Token> IssueTokenAsync(string userId);

    ValueTask<Token> LoginAsync(string username, string password);

    ValueTask LogoutAsync();

    ValueTask<(SSOUser, string)> RegisterAsync(RegisterUser registerForm);

    ValueTask ChangePasswordAsync(string username, string oldPassword, string newPassword);

    ValueTask<Token> ForgotPasswordAsync(string email);

    ValueTask ConfirmRegistrationAsync(string token);

    ValueTask ConfirmForgotPasswordAsync(string token, string userId, string newPassword, string confirmNewPassword);

    ValueTask<(SSOUser, string)> InviteUserAsync(RegisterUser registerForm);

    ValueTask<SSOUser> AcceptInviteAsync(RegisterUser user, string userId, string inviteToken);

    ValueTask<string> RegenerateUserInviteToken(string userId);
}
