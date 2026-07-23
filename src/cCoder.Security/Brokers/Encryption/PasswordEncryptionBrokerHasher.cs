// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Identity;

namespace cCoder.Security.Brokers.Encryption;

internal class PasswordEncryptionBrokerHasher : IPasswordEncryptionBroker
{
    private readonly PasswordHasher<object> hasher = new();

    public string Encrypt(string password) =>
        hasher.HashPassword(user: new object(), password: password);

    public bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText) =>
        hasher.VerifyHashedPassword(user: new object(), hashedPassword: encrypted, providedPassword: plainText) is
            PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}