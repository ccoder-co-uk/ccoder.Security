// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
namespace cCoder.Security.Data.Dependencies;

public partial class AesCrypto<T>(string decryptionKey)
    : IDataProtector
{
    IDataProtector IDataProtectionProvider.CreateProtector(
        string purpose)
    {
        ArgumentException.ThrowIfNullOrEmpty(argument: purpose);

        return new AesCrypto<T>(
            decryptionKey: $"{decryptionKey}:{purpose}");
    }

    public string Encrypt(T source, string key)
    {
        Encoding e = Encoding.UTF8;
        byte[] rawData = e.GetBytes(s: JsonSerializer.Serialize(value: source));
        byte[] cipherData = SimpleEncryptWithPassword(secretMessage: rawData, password: key, nonSecretPayload: null);
        return Convert.ToBase64String(inArray: cipherData);
    }

    public string Encrypt(T source)
    {
        if (decryptionKey == null)
        {
            throw new InvalidOperationException("Decryption key not set.");
        }

        Encoding e = Encoding.UTF8;
        byte[] rawData = e.GetBytes(s: System.Text.Json.JsonSerializer.Serialize(value: source));
        byte[] cipherData = SimpleEncryptWithPassword(secretMessage: rawData, password: decryptionKey, nonSecretPayload: null);
        return Convert.ToBase64String(inArray: cipherData);
    }

    public T Decrypt(string source, string key)
    {
        Encoding e = Encoding.UTF8;
        byte[] decryptedBytes = SimpleDecryptWithPassword(encryptedMessage: Convert.FromBase64String(s: source), password: key);
        return JsonSerializer.Deserialize<T>(json: e.GetString(bytes: decryptedBytes));
    }

    public T Decrypt(string source)
    {
        if (decryptionKey == null)
        {
            throw new InvalidOperationException("Decryption key not set.");
        }

        Encoding e = Encoding.UTF8;
        byte[] decryptedBytes = SimpleDecryptWithPassword(encryptedMessage: Convert.FromBase64String(s: source), password: decryptionKey);
        return JsonSerializer.Deserialize<T>(json: e.GetString(bytes: decryptedBytes));
    }

    byte[] IDataProtector.Protect(byte[] plaintext) =>
        SimpleEncryptWithPassword(
            secretMessage: plaintext,
            password: decryptionKey,
            nonSecretPayload: null);

    byte[] IDataProtector.Unprotect(byte[] protectedData) =>
        SimpleDecryptWithPassword(
            encryptedMessage: protectedData,
            password: decryptionKey);
}