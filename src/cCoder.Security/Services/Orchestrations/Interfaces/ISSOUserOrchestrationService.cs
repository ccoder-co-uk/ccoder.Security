// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

public interface ISSOUserOrchestrationService
{
    ValueTask<(SSOUser, string)> RegisterUserAsync(RegisterUser registerForm);

    ValueTask<(SSOUser, string)> InviteRegisterUserAsync(RegisterUser registerForm);

    ValueTask<SSOUser> AcceptRegisterUserInviteAsync(
        RegisterUser registerForm,
        string userId,
        string tokenId);

    ValueTask<string> RegenerateUserInviteToken(string userId);

    ValueTask ConfirmRegistration(string tokenId);

    ValueTask<SSOUser> UpdateSSOUserAsync(string username, SSOUser item);

    ValueTask DeleteSSOUserAsync(SSOUser item);

    IQueryable<SSOUser> GetAllSSOUsers();
}