// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class SSOUserAggregationService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSSOUsersOnGet() =>
        Validate(inputs: []);

    private static void ValidateRegistrationOnRegister(
        RegisterUser registerForm) =>
        Validate(inputs: registerForm);

    private static void ValidateSSOUserOnUpdate(
        string username,
        SSOUser updatedSSOUser) =>
        Validate(inputs: [username, updatedSSOUser]);

    private static void ValidateSSOUserOnDelete(SSOUser deletedSSOUser) =>
        Validate(inputs: deletedSSOUser);

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