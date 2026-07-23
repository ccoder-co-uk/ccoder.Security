// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Aggregations.Interfaces;

public interface IRegistrationAggregationService
{
    ValueTask<RegisterUser> RegisterUserAsync(RegisterUser registerForm);

    ValueTask<RegisterUser> InviteRegisterUserAsync(RegisterUser registerForm);

    ValueTask<RegisterUser> AcceptRegisterUserInviteAsync(
        RegisterUser registerForm,
        string userId,
        string tokenId);

    ValueTask<string> RegenerateUserInviteToken(string userId);

    ValueTask ConfirmRegistration(string tokenId);

    ValueTask SetupRegisterUserAsync(RegisterUser newRegisterUser);
}