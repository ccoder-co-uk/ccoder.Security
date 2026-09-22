// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class RegistrationManager(
    IRegistrationAggregationService registrationAggregationService)
        : IRegistrationManager
{
    public ValueTask<RegisterUser> RegisterUserAsync(RegisterUser registerUser) =>
        registrationAggregationService.RegisterUserAsync(
            registerForm: registerUser);

    public ValueTask<RegisterUser> InviteRegisterUserAsync(RegisterUser registerUser) =>
        registrationAggregationService.InviteRegisterUserAsync(
            registerForm: registerUser);

    public ValueTask<RegisterUser> AcceptRegisterUserInviteAsync(
        RegisterUser registerUser,
        string userId,
        string tokenId) =>
        registrationAggregationService.AcceptRegisterUserInviteAsync(
            registerForm: registerUser,
            userId: userId,
            tokenId: tokenId);

    public ValueTask<string> RegenerateUserInviteToken(string userId) =>
        registrationAggregationService.RegenerateUserInviteToken(
            userId: userId);

    public ValueTask ConfirmRegistration(string tokenId) =>
        registrationAggregationService.ConfirmRegistration(tokenId: tokenId);

    public ValueTask SetupRegisterUserAsync(RegisterUser registerUser) =>
        registrationAggregationService.SetupRegisterUserAsync(
            newRegisterUser: registerUser);

}