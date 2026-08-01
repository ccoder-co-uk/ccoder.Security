// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;

namespace cCoder.Security.Dependencies.Encryption;

internal interface IPasswordHashingDependency
{
    string HashPassword(string password);

    PasswordVerificationOutcome VerifyHashedPassword(
        string hashedPassword,
        string providedPassword);

    void PerformDummyVerification(string providedPassword);

    string HashTokenSecret(string secret);

    bool VerifyTokenSecret(string secretHash, string providedSecret);
}