// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data;

namespace cCoder.Security.Brokers.Encryption;

internal sealed class LegacyPasswordEncryptionBroker(
    ISymmetricCrypto<string> crypto)
    : ILegacyPasswordEncryptionBroker
{
    public string Decrypt(string encryptedPassword) =>
        crypto.Decrypt(source: encryptedPassword);
}