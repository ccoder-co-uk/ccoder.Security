// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace cCoder.Security.Brokers.Encryption;

internal class PasswordEncryptionBrokerSHA512 : IPasswordEncryptionBroker
{
    public string Encrypt(string password)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(s: password);
        byte[] hashBytes = SHA512.HashData(source: inputBytes);
        return Encoding.UTF8.GetString(bytes: hashBytes);
    }

    public bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(s: plainText);
        byte[] hashBytes = SHA512.HashData(source: inputBytes);
        string hashedString = Encoding.UTF8.GetString(bytes: hashBytes);
        return hashedString == encrypted;
    }
}