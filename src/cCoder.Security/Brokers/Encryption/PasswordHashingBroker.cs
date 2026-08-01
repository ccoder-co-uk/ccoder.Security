// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Dependencies.Encryption;
using cCoder.Security.Models;

namespace cCoder.Security.Brokers.Encryption;

internal sealed class PasswordHashingBroker(
    IPasswordHashingDependency passwordHashingDependency)
    : IPasswordHashingBroker
{
    public string HashPassword(string password) =>
        passwordHashingDependency.HashPassword(password: password);

    public PasswordVerificationOutcome VerifyHashedPassword(
        string hashedPassword,
        string providedPassword) =>
        passwordHashingDependency.VerifyHashedPassword(
            hashedPassword: hashedPassword,
            providedPassword: providedPassword);

    public void PerformDummyVerification(string providedPassword) =>
        passwordHashingDependency.PerformDummyVerification(
            providedPassword: providedPassword);

    public string HashTokenSecret(string secret) =>
        passwordHashingDependency.HashTokenSecret(secret: secret);

    public bool VerifyTokenSecret(
        string secretHash,
        string providedSecret) =>
        passwordHashingDependency.VerifyTokenSecret(
            secretHash: secretHash,
            providedSecret: providedSecret);
}