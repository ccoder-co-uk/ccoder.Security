// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using System.Text.Json;
using cCoder.Security.Data.Dependencies;

namespace cCoder.Security.Data.Dependencies;

public class AesCrypto<T>(string decryptionKey) : ISymmetricCrypto<T>
{
    private readonly AesThenHmac crypto = new();

    public string Encrypt(T source, string key)
    {
        Encoding e = Encoding.UTF8;
        byte[] rawData = e.GetBytes(s: JsonSerializer.Serialize(source));
        byte[] cipherData = crypto.SimpleEncryptWithPassword(secretMessage: rawData, password: key, nonSecretPayload: null);
        return Convert.ToBase64String(inArray: cipherData);
    }

    public string Encrypt(T source)
    {
        if (decryptionKey == null)
        {
            throw new InvalidOperationException("Decryption key not set.");
        }

        Encoding e = Encoding.UTF8;
        byte[] rawData = e.GetBytes(s: System.Text.Json.JsonSerializer.Serialize(source));
        byte[] cipherData = crypto.SimpleEncryptWithPassword(secretMessage: rawData, password: decryptionKey, nonSecretPayload: null);
        return Convert.ToBase64String(inArray: cipherData);
    }

    public T Decrypt(string source, string key)
    {
        Encoding e = Encoding.UTF8;
        byte[] decryptedBytes = crypto.SimpleDecryptWithPassword(encryptedMessage: Convert.FromBase64String(source), password: key);
        return JsonSerializer.Deserialize<T>(json: e.GetString(decryptedBytes));
    }

    public T Decrypt(string source)
    {
        if (decryptionKey == null)
        {
            throw new InvalidOperationException("Decryption key not set.");
        }

        Encoding e = Encoding.UTF8;
        byte[] decryptedBytes = crypto.SimpleDecryptWithPassword(encryptedMessage: Convert.FromBase64String(source), password: decryptionKey);
        return JsonSerializer.Deserialize<T>(json: e.GetString(decryptedBytes));
    }
}