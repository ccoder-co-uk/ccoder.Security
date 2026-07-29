// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class RegistrationAggregationService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateRegistrationOnRegister(
        RegisterUser registerForm) =>
        Validate(inputs: registerForm);

    private static void ValidateRegistrationOnInvite(
        RegisterUser registerForm) =>
        Validate(inputs: registerForm);

    private static void ValidateRegistrationOnAccept(
        RegisterUser registerForm,
        string userId,
        string tokenId) =>
        Validate(inputs: [registerForm, userId, tokenId]);

    private static void ValidateInvitationTokenOnRegenerate(string userId) =>
        Validate(inputs: userId);

    private static void ValidateRegistrationOnConfirm(string tokenId) =>
        Validate(inputs: tokenId);
}