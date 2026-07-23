// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class AuthenticationAggregationService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateTokenOnIssue(
        string userId,
        TokenUse tokenUse) =>
        Validate(inputs: [userId, tokenUse]);

    private static void ValidateCredentialsOnLogin(
        string username,
        string password) =>
        Validate(inputs: [username, password]);

    private static void ValidateAuthenticationOnLogout() =>
        Validate(inputs: []);

    private static void ValidatePasswordOnChange(
        string username,
        string oldPassword,
        string newPassword) =>
        Validate(inputs: [username, oldPassword, newPassword]);

    private static void ValidatePasswordOnForget(string email) =>
        Validate(inputs: email);

    private static void ValidateForgottenPasswordOnConfirm(
        string tokenId,
        string userId,
        string newPassword,
        string confirmNewPassword) =>
        Validate(inputs:
        [
            tokenId,
            userId,
            newPassword,
            confirmNewPassword
        ]);
}