// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SSOUserProcessingService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private void ValidateSSOUserOnRegister(SSOUser user)
    {
        Validate(inputs: user);
        EnsureSSOUserIsValid(user: user, validatePassword: true);
    }

    private void ValidateSSOUserOnInvite(SSOUser user)
    {
        Validate(inputs: user);
        EnsureSSOUserIsValid(user: user, validatePassword: false);
    }

    private static void ValidateSSOUserOnDelete(SSOUser deletedSSOUser) =>
        Validate(inputs: deletedSSOUser);

    private static void ValidateCredentialsOnFind(string username, string password) =>
        Validate(inputs: [username, password]);

    private static void ValidateSSOUserOnFind(string ssoUserId) =>
        Validate(inputs: ssoUserId);

    private static void ValidateSSOUsersOnGet(bool ignoreFilters) =>
        Validate(inputs: ignoreFilters);

    private static void ValidateSSOUserOnUpdate(SSOUser updatedSSOUser) =>
        Validate(inputs: updatedSSOUser);

    private static void ValidateSSOUserOnGetCurrent() =>
        Validate(inputs: []);

    private static void ValidatePasswordInput(string password) =>
        Validate(inputs: password);

    private void EnsureSSOUserIsValid(SSOUser user, bool validatePassword)
    {
        if (!user.Email.Contains(value: '@'))
        {
            throw new ValidationException("Invalid email provided");
        }

        if (string.IsNullOrEmpty(value: user.DisplayName))
        {
            throw new ValidationException("Display name cannot be empty");
        }

        if (validatePassword && string.IsNullOrEmpty(value: user.PasswordHash))
        {
            throw new ValidationException("Password cannot be empty");
        }

        bool emailInSystem = ssoUserService
            .GetAllSSOUsers(ignoreFilters: true)
            .Any(predicate: ssoUser => ssoUser.Email == user.Email);

        if (emailInSystem)
        {
            throw new ValidationException("Email exists");
        }

        if (validatePassword)
        {
            EnsurePasswordIsValid(password: user.PasswordHash);
        }
    }

    private static void EnsurePasswordIsValid(string password)
    {
        if (password.Length < 8)
        {
            throw new ValidationException("Password is too short");
        }

        bool passwordHasLetters = password.Any(predicate: character =>
            char.IsLetter(c: character));

        bool passwordHasDigits = password.Any(predicate: character =>
            char.IsNumber(c: character));

        bool passwordHasUpperCase = password.Any(predicate: character =>
            char.IsUpper(c: character));

        bool passwordHasLowerCase = password.Any(predicate: character =>
            char.IsLower(c: character));

        if (!(passwordHasLetters && passwordHasDigits))
        {
            throw new ValidationException(
                "Password must contain both letter and numbers.");
        }

        if (!(passwordHasUpperCase && passwordHasLowerCase))
        {
            throw new ValidationException(
                "Password must contain uppercase and lower case characters.");
        }
    }
}