// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace cCoder.Security.Dependencies.Encryption;

internal sealed class PasswordHashingDependency
    : IPasswordHasher<object>
{
    internal const int MinimumMemorySizeInKilobytes = 19_456;
    internal const int MinimumIterations = 2;
    internal const int MinimumDegreeOfParallelism = 1;
    internal const int MinimumSaltSizeInBytes = 16;
    internal const int MinimumHashSizeInBytes = 32;
    private const string Prefix = "$argon2id$";
    private readonly int memorySizeInKilobytes;
    private readonly int iterations;
    private readonly int degreeOfParallelism;
    private readonly int saltSizeInBytes;
    private readonly int hashSizeInBytes;
    private readonly PasswordHasher<object> identityHasher;
    private readonly string dummyHash;

    public PasswordHashingDependency(
        int memorySizeInKilobytes,
        int iterations,
        int degreeOfParallelism,
        int saltSizeInBytes,
        int hashSizeInBytes)
    {
        ValidateConfiguration(
            memorySizeInKilobytes: memorySizeInKilobytes,
            iterations: iterations,
            degreeOfParallelism: degreeOfParallelism,
            saltSizeInBytes: saltSizeInBytes,
            hashSizeInBytes: hashSizeInBytes);

        this.memorySizeInKilobytes = memorySizeInKilobytes;
        this.iterations = iterations;
        this.degreeOfParallelism = degreeOfParallelism;
        this.saltSizeInBytes = saltSizeInBytes;
        this.hashSizeInBytes = hashSizeInBytes;
        identityHasher = CreateIdentityMigrationHasher();
        dummyHash = HashPassword(
            user: new object(),
            password: "cCoder timing sentinel password");
    }

    public string HashPassword(object user, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(
            count: saltSizeInBytes);

        byte[] hash = DeriveHash(
            password: password,
            salt: salt,
            memorySizeInKilobytes: memorySizeInKilobytes,
            iterations: iterations,
            degreeOfParallelism: degreeOfParallelism,
            hashSizeInBytes: hashSizeInBytes);

        return $"{Prefix}v=19$m={memorySizeInKilobytes},t={iterations},p={degreeOfParallelism}${Convert.ToBase64String(inArray: salt)}${Convert.ToBase64String(inArray: hash)}";
    }

    public PasswordVerificationResult VerifyHashedPassword(
        object user,
        string hashedPassword,
        string providedPassword)
    {
        if (!hashedPassword.StartsWith(
            value: Prefix,
            comparisonType: StringComparison.Ordinal))
        {
            PasswordVerificationResult identityResult =
                identityHasher.VerifyHashedPassword(
                    user: new object(),
                    hashedPassword: hashedPassword,
                    providedPassword: providedPassword);

            return identityResult == PasswordVerificationResult.Failed
                ? PasswordVerificationResult.Failed
                : PasswordVerificationResult.SuccessRehashNeeded;
        }

        if (!TryParseHash(
            encodedHash: hashedPassword,
            hashParameters: out var hashParameters))
        {
            return PasswordVerificationResult.Failed;
        }

        byte[] providedHash = DeriveHash(
            password: providedPassword,
            salt: hashParameters.Salt,
            memorySizeInKilobytes: hashParameters.MemorySizeInKilobytes,
            iterations: hashParameters.Iterations,
            degreeOfParallelism: hashParameters.DegreeOfParallelism,
            hashSizeInBytes: hashParameters.Hash.Length);

        if (!CryptographicOperations.FixedTimeEquals(
            left: hashParameters.Hash,
            right: providedHash))
        {
            return PasswordVerificationResult.Failed;
        }

        return MeetsCurrentConfiguration(hashParameters: hashParameters)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.SuccessRehashNeeded;
    }

    public void PerformDummyVerification(string providedPassword) =>
        VerifyHashedPassword(
            user: new object(),
            hashedPassword: dummyHash,
            providedPassword: providedPassword);

    public string HashTokenSecret(string secret) =>
        Convert.ToHexString(
            inArray: SHA256.HashData(
                source: Encoding.UTF8.GetBytes(s: secret)));

    public bool VerifyTokenSecret(string secretHash, string providedSecret) =>
        CryptographicOperations.FixedTimeEquals(
            left: Convert.FromHexString(s: secretHash),
            right: SHA256.HashData(
                source: Encoding.UTF8.GetBytes(s: providedSecret)));

    private static byte[] DeriveHash(
        string password,
        byte[] salt,
        int memorySizeInKilobytes,
        int iterations,
        int degreeOfParallelism,
        int hashSizeInBytes)
    {
        using Argon2id argon = new(
            password: Encoding.UTF8.GetBytes(s: password))
        {
            Salt = salt,
            MemorySize = memorySizeInKilobytes,
            Iterations = iterations,
            DegreeOfParallelism = degreeOfParallelism
        };

        return argon.GetBytes(bc: hashSizeInBytes);
    }

    private static void ValidateConfiguration(
        int memorySizeInKilobytes,
        int iterations,
        int degreeOfParallelism,
        int saltSizeInBytes,
        int hashSizeInBytes)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(
            value: memorySizeInKilobytes,
            other: MinimumMemorySizeInKilobytes);

        ArgumentOutOfRangeException.ThrowIfLessThan(
            value: iterations,
            other: MinimumIterations);

        ArgumentOutOfRangeException.ThrowIfLessThan(
            value: degreeOfParallelism,
            other: MinimumDegreeOfParallelism);

        ArgumentOutOfRangeException.ThrowIfLessThan(
            value: saltSizeInBytes,
            other: MinimumSaltSizeInBytes);

        ArgumentOutOfRangeException.ThrowIfLessThan(
            value: hashSizeInBytes,
            other: MinimumHashSizeInBytes);
    }

    private static PasswordHasher<object> CreateIdentityMigrationHasher() =>
        new(optionsAccessor: Options.Create(
            options: new PasswordHasherOptions
            {
                CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
                IterationCount = 220_000
            }));

    private bool MeetsCurrentConfiguration(
        (int MemorySizeInKilobytes,
         int Iterations,
         int DegreeOfParallelism,
         byte[] Salt,
         byte[] Hash) hashParameters) =>
        hashParameters.MemorySizeInKilobytes
            >= memorySizeInKilobytes
        && hashParameters.Iterations >= iterations
        && hashParameters.DegreeOfParallelism
            >= degreeOfParallelism
        && hashParameters.Salt.Length >= saltSizeInBytes
        && hashParameters.Hash.Length >= hashSizeInBytes;

    private static bool TryParseHash(
        string encodedHash,
        out (int MemorySizeInKilobytes,
             int Iterations,
             int DegreeOfParallelism,
             byte[] Salt,
             byte[] Hash) hashParameters)
    {
        hashParameters = default;

        try
        {
            string[] sections = encodedHash.Split(separator: '$');

            if (sections.Length != 6
                || sections[1] != "argon2id"
                || sections[2] != "v=19")
            {
                return false;
            }

            string[] parameters = sections[3].Split(separator: ',');

            if (parameters.Length != 3)
            {
                return false;
            }

            int memorySizeInKilobytes = ParseParameter(
                parameter: parameters[0],
                name: "m");

            int iterations = ParseParameter(
                parameter: parameters[1],
                name: "t");

            int degreeOfParallelism = ParseParameter(
                parameter: parameters[2],
                name: "p");

            byte[] salt = Convert.FromBase64String(s: sections[4]);
            byte[] hash = Convert.FromBase64String(s: sections[5]);

            if (memorySizeInKilobytes <= 0
                || iterations <= 0
                || degreeOfParallelism <= 0
                || salt.Length == 0
                || hash.Length == 0)
            {
                return false;
            }

            hashParameters = (
                MemorySizeInKilobytes: memorySizeInKilobytes,
                Iterations: iterations,
                DegreeOfParallelism: degreeOfParallelism,
                Salt: salt,
                Hash: hash);

            return true;
        }
        catch (Exception exception) when (
            exception is FormatException
            or OverflowException)
        {
            return false;
        }
    }

    private static int ParseParameter(string parameter, string name)
    {
        string expectedPrefix = name + "=";

        if (!parameter.StartsWith(
            value: expectedPrefix,
            comparisonType: StringComparison.Ordinal))
        {
            throw new FormatException(
                message: $"Expected Argon2 parameter '{name}'.");
        }

        return int.Parse(
            s: parameter[expectedPrefix.Length..],
            provider: CultureInfo.InvariantCulture);
    }
}