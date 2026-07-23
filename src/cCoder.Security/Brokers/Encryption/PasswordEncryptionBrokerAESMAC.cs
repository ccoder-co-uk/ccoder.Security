// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data;

namespace cCoder.Security.Brokers.Encryption;

internal class PasswordEncryptionBrokerAESHMAC(ISymmetricCrypto<string> crypto)
    : IPasswordEncryptionBroker
{
    public string Encrypt(string password) =>
        crypto.Encrypt(source: password);

    public bool EncryptedAndPlainTextAreEqual(string encrypted, string plainText) =>
        crypto.Decrypt(source: encrypted) == plainText;
}