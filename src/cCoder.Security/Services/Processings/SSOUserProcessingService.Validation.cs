// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Security.Services.Processings;

internal partial class SSOUserProcessingService
{
    public void ValidateSSOUser(SSOUser user, bool validatePassword = true)
    {
        if (!user.Email.Contains(value: '@'))
        { throw new ValidationException("Invalid email provided"); }

        if (string.IsNullOrEmpty(value: user.DisplayName))
        { throw new ValidationException("Display name cannot be empty"); }

        if (validatePassword && string.IsNullOrEmpty(value: user.PasswordHash))
        { throw new ValidationException("Password cannot be empty"); }

        bool emailInSystem = ssoUserService
            .GetAllSSOUsers(true)
            .Any(predicate: sso => sso.Email == user.Email);

        if (emailInSystem)
        { throw new ValidationException("Email exists"); }

        if (validatePassword)
        { ValidatePassword(password: user.PasswordHash); }
    }

    public void ValidatePassword(string password)
    {
        if (password.Length < 8)
        { throw new ValidationException("Password is too short"); }

        bool passwordHasLetters = password.Any(predicate: c => char.IsLetter(c));
        bool passwordHasDigits = password.Any(predicate: c => char.IsNumber(c));
        bool passwordHasUpperCase = password.Any(predicate: c => char.IsUpper(c));
        bool passwordHasLowerCase = password.Any(predicate: c => char.IsLower(c));

        if (!(passwordHasLetters && passwordHasDigits))
        { throw new ValidationException("Password must contain both letter and numbers."); }

        if (!(passwordHasUpperCase && passwordHasLowerCase))
        { throw new ValidationException("Password must contain uppercase and lower case characters."); }
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrEmpty(value: username))
        { throw new ValidationException("User cannot be empty!"); }
    }
}