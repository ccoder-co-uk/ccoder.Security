// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies.Encryption;
using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Xunit;

namespace cCoder.Security.Tests.Dependencies.Encryption;

public sealed partial class PasswordHashingDependencyTests
{
    private const string Password = "Correct Horse Battery Staple 123";
    private readonly PasswordHashingDependency broker = new(
        configuration: new ArgonConfiguration());

    [Fact]
    public void ShouldConfigureArgonAtOrAboveOwaspMinimums()
    {
        // Given
        ArgonConfiguration configuration = new();

        // When
        Action action = () => _ = new PasswordHashingDependency(
            configuration: configuration);

        // Then
        configuration.MemorySizeInKilobytes
            .Should()
            .BeGreaterThanOrEqualTo(
                expected: PasswordHashingDependency
                    .MinimumMemorySizeInKilobytes);

        configuration.Iterations
            .Should()
            .BeGreaterThanOrEqualTo(
                expected: PasswordHashingDependency.MinimumIterations);

        configuration.DegreeOfParallelism
            .Should()
            .BeGreaterThanOrEqualTo(
                expected: PasswordHashingDependency
                    .MinimumDegreeOfParallelism);

        configuration.SaltSizeInBytes
            .Should()
            .BeGreaterThanOrEqualTo(
                expected: PasswordHashingDependency
                    .MinimumSaltSizeInBytes);

        configuration.HashSizeInBytes
            .Should()
            .BeGreaterThanOrEqualTo(
                expected: PasswordHashingDependency
                    .MinimumHashSizeInBytes);

        action
            .Should()
            .NotThrow();
    }

    [Fact]
    public void ShouldRejectArgonConfigurationBelowOwaspMinimums()
    {
        // Given
        ArgonConfiguration configuration = new()
        {
            MemorySizeInKilobytes = PasswordHashingDependency
                .MinimumMemorySizeInKilobytes - 1
        };

        // When
        Action action = () => _ = new PasswordHashingDependency(
            configuration: configuration);

        // Then
        action
            .Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ShouldSaltIdenticalPasswordsIndependently()
    {
        // Given

        // When
        string firstHash = broker.HashPassword(password: Password);
        string secondHash = broker.HashPassword(password: Password);

        // Then
        firstHash
            .Should()
            .NotBe(unexpected: secondHash);

        firstHash
            .Should()
            .NotContain(unexpected: Password);

        secondHash
            .Should()
            .NotContain(unexpected: Password);
    }

    [Fact]
    public void ShouldVerifyOnlyTheCorrectPassword()
    {
        // Given
        string hash = broker.HashPassword(password: Password);

        // When
        PasswordVerificationOutcome correctResult =
            broker.VerifyHashedPassword(
                hashedPassword: hash,
                providedPassword: Password);

        PasswordVerificationOutcome incorrectResult =
            broker.VerifyHashedPassword(
                hashedPassword: hash,
                providedPassword: "Incorrect Password 123");

        // Then
        correctResult
            .Should()
            .Be(expected: PasswordVerificationOutcome.Success);

        incorrectResult
            .Should()
            .Be(expected: PasswordVerificationOutcome.Failed);
    }

    [Fact]
    public void ShouldRequestRehashForLowerWorkFactorIdentityHash()
    {
        // Given
        PasswordHasher<object> legacyHasher = new(
            optionsAccessor: Options.Create(
                options: new PasswordHasherOptions
                {
                    CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
                    IterationCount = 100_000
                }));

        string legacyHash = legacyHasher.HashPassword(
            user: new object(),
            password: Password);

        // When
        PasswordVerificationOutcome result =
            broker.VerifyHashedPassword(
                hashedPassword: legacyHash,
                providedPassword: Password);

        // Then
        result
            .Should()
            .Be(expected: PasswordVerificationOutcome.SuccessRehashNeeded);
    }

    [Fact]
    public void ShouldPerformUnknownAccountVerificationWork()
    {
        // Given
        Action action = () => broker.PerformDummyVerification(
            providedPassword: Password);

        // When
        Exception exception = Record.Exception(testCode: action);

        // Then
        exception
            .Should()
            .BeNull();
    }

    [Fact]
    public void ShouldHashAndVerifyHighEntropyTokenSecrets()
    {
        // Given
        string secret = Convert.ToBase64String(
            inArray: System.Security.Cryptography.RandomNumberGenerator.GetBytes(
                count: 32));

        // When
        string secretHash = broker.HashTokenSecret(secret: secret);

        bool correctSecretMatches = broker.VerifyTokenSecret(
            secretHash: secretHash,
            providedSecret: secret);

        bool incorrectSecretMatches = broker.VerifyTokenSecret(
            secretHash: secretHash,
            providedSecret: secret + "invalid");

        // Then
        secretHash
            .Should()
            .NotContain(unexpected: secret);

        correctSecretMatches
            .Should()
            .BeTrue();

        incorrectSecretMatches
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData("$argon2id$v=18$m=19456,t=2,p=1$AA==$AA==")]
    [InlineData("$argon2id$v=19$m=bad,t=2,p=1$AA==$AA==")]
    [InlineData("$argon2id$v=19$m=19456,x=2,p=1$AA==$AA==")]
    [InlineData("$argon2id$v=19$m=19456,t=2,p=1$not-base64$AA==")]
    [InlineData("$argon2id$v=19$m=0,t=2,p=1$AA==$AA==")]
    public void ShouldRejectMalformedArgonHashes(string malformedHash)
    {
        // Given

        // When

        PasswordVerificationOutcome result = broker.VerifyHashedPassword(
            hashedPassword: malformedHash,
            providedPassword: Password);

        // Then

        result
            .Should()
            .Be(expected: PasswordVerificationOutcome.Failed);
    }
}