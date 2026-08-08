// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Dependencies;
using FluentAssertions;
using System.Text;
using Xunit;

namespace cCoder.Security.Tests.Encryption;

public partial class AesCoverageTests
{
    private const string Password = "CoveragePassword123!";

    [Fact]
    public void ShouldRoundTripMessagesWithBinaryKeys()
    {
        // Given

        const string message = "coverage-message";
        AesThenHmac crypto = new();
        byte[] cryptKey = crypto.NewKey();
        byte[] authKey = crypto.NewKey();
        byte[] payload = [1, 2, 3];

        // When

        string encryptedText = crypto.SimpleEncrypt(
            secretMessage: message,
            cryptKey: cryptKey,
            authKey: authKey,
            nonSecretPayload: payload);

        string decryptedText = crypto.SimpleDecrypt(
            encryptedMessage: encryptedText,
            cryptKey: cryptKey,
            authKey: authKey,
            nonSecretPayloadLength: payload.Length);

        byte[] messageBytes = Encoding.UTF8.GetBytes(s: message);

        byte[] encryptedBytes = crypto.SimpleEncrypt(
            secretMessage: messageBytes,
            cryptKey: cryptKey,
            authKey: authKey,
            nonSecretPayload: null);

        byte[] decryptedBytes = crypto.SimpleDecrypt(
            encryptedMessage: encryptedBytes,
            cryptKey: cryptKey,
            authKey: authKey);

        // Then

        decryptedText
            .Should()
            .Be(expected: message);

        decryptedBytes
            .Should()
            .Equal(expected: messageBytes);
    }

    [Fact]
    public void ShouldRoundTripMessagesWithPasswordDerivedKeys()
    {
        // Given

        const string message = "coverage-message";
        AesThenHmac crypto = new();
        byte[] payload = [4, 5, 6];

        // When

        string encryptedText = crypto.SimpleEncryptWithPassword(
            secretMessage: message,
            password: Password,
            nonSecretPayload: payload);

        string decryptedText = crypto.SimpleDecryptWithPassword(
            encryptedMessage: encryptedText,
            password: Password,
            nonSecretPayloadLength: payload.Length);

        byte[] messageBytes = Encoding.UTF8.GetBytes(s: message);

        byte[] encryptedBytes = crypto.SimpleEncryptWithPassword(
            secretMessage: messageBytes,
            password: Password,
            nonSecretPayload: null);

        byte[] decryptedBytes = crypto.SimpleDecryptWithPassword(
            encryptedMessage: encryptedBytes,
            password: Password);

        // Then

        decryptedText
            .Should()
            .Be(expected: message);

        decryptedBytes
            .Should()
            .Equal(expected: messageBytes);
    }

    [Fact]
    public void ShouldRejectTamperedAndTruncatedCipherText()
    {
        // Given

        AesThenHmac crypto = new();
        byte[] cryptKey = crypto.NewKey();
        byte[] authKey = crypto.NewKey();

        byte[] encrypted = crypto.SimpleEncrypt(
            secretMessage: Encoding.UTF8.GetBytes(s: "coverage-message"),
            cryptKey: cryptKey,
            authKey: authKey,
            nonSecretPayload: null);

        encrypted[^1] ^= 1;

        // When

        byte[] tamperedResult = crypto.SimpleDecrypt(
            encryptedMessage: encrypted,
            cryptKey: cryptKey,
            authKey: authKey);

        Action decryptTruncated = () => crypto.SimpleDecrypt(
            encryptedMessage: [1],
            cryptKey: cryptKey,
            authKey: authKey);

        // Then

        tamperedResult
            .Should()
            .BeEmpty();

        decryptTruncated
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldValidateEveryEncryptionInput()
    {
        // Given

        AesThenHmac crypto = new();
        byte[] key = crypto.NewKey();
        byte[] invalidKey = [1];

        Action[] invalidOperations =
        [
            () => crypto.SimpleEncrypt(
                secretMessage: string.Empty,
                cryptKey: key,
                authKey: key,
                nonSecretPayload: null),
            () => crypto.SimpleEncrypt(
                secretMessage: [1],
                cryptKey: invalidKey,
                authKey: key,
                nonSecretPayload: null),
            () => crypto.SimpleEncrypt(
                secretMessage: [1],
                cryptKey: key,
                authKey: invalidKey,
                nonSecretPayload: null),
            () => crypto.SimpleEncrypt(
                secretMessage: (byte[])null,
                cryptKey: key,
                authKey: key,
                nonSecretPayload: null),
            () => crypto.SimpleDecrypt(
                encryptedMessage: string.Empty,
                cryptKey: key,
                authKey: key),
            () => crypto.SimpleDecrypt(
                encryptedMessage: [1],
                cryptKey: invalidKey,
                authKey: key),
            () => crypto.SimpleDecrypt(
                encryptedMessage: [1],
                cryptKey: key,
                authKey: invalidKey),
            () => crypto.SimpleDecrypt(
                encryptedMessage: (byte[])null,
                cryptKey: key,
                authKey: key),
            () => crypto.SimpleEncryptWithPassword(
                secretMessage: string.Empty,
                password: Password,
                nonSecretPayload: null),
            () => crypto.SimpleEncryptWithPassword(
                secretMessage: [1],
                password: "short",
                nonSecretPayload: null),
            () => crypto.SimpleEncryptWithPassword(
                secretMessage: (byte[])null,
                password: Password,
                nonSecretPayload: null),
            () => crypto.SimpleDecryptWithPassword(
                encryptedMessage: string.Empty,
                password: Password),
            () => crypto.SimpleDecryptWithPassword(
                encryptedMessage: [1],
                password: "short"),
            () => crypto.SimpleDecryptWithPassword(
                encryptedMessage: (byte[])null,
                password: Password)
        ];

        // When

        Exception[] failures = invalidOperations
            .Select(selector: operation => Record.Exception(testCode: operation))
            .ToArray();

        // Then

        failures
            .Should()
            .OnlyContain(predicate: failure => failure is ArgumentException);
    }

    [Fact]
    public void ShouldRoundTripTypedValuesAndRequireDefaultKey()
    {
        // Given

        const string value = "coverage-value";
        AesCrypto<string> configured = new(decryptionKey: Password);
        AesCrypto<string> unconfigured = new(decryptionKey: null);

        // When

        string defaultCipher = configured.Encrypt(source: value);
        string defaultValue = configured.Decrypt(source: defaultCipher);
        string explicitCipher = configured.Encrypt(source: value, key: Password);

        string explicitValue = configured.Decrypt(
            source: explicitCipher,
            key: Password);

        Action encryptWithoutKey = () => unconfigured.Encrypt(source: value);
        Action decryptWithoutKey = () => unconfigured.Decrypt(source: defaultCipher);

        // Then

        defaultValue
            .Should()
            .Be(expected: value);

        explicitValue
            .Should()
            .Be(expected: value);

        encryptWithoutKey
            .Should()
            .Throw<InvalidOperationException>();

        decryptWithoutKey
            .Should()
            .Throw<InvalidOperationException>();
    }
}