// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.Encryption.Interfaces;

internal interface IPasswordHashingBroker : IUtilityBroker
{
    string HashPassword(string password);

    PasswordVerificationOutcome VerifyHashedPassword(
        string hashedPassword,
        string providedPassword);

    void PerformDummyVerification(string providedPassword);

    string HashTokenSecret(string secret);

    bool VerifyTokenSecret(string secretHash, string providedSecret);
}