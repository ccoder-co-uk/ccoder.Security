// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

public interface ISSOUserOrchestrationService
{
    ValueTask<(SSOUser, string)> Register(RegisterUser newRegisterUser);

    ValueTask<(SSOUser, string)> InviteUserAsync(RegisterUser newRegisterUser);

    ValueTask<SSOUser> AcceptInviteAsync(RegisterUser registerForm, string userId, string tokenId);

    ValueTask<string> RegenerateUserInviteToken(string userId);

    ValueTask ConfirmRegistration(string tokenId);

    ValueTask<SSOUser> UpdateSSOUserAsync(string username, SSOUser updatedSSOUser);

    ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser);

    IQueryable<SSOUser> GetAllSSOUsers();
}