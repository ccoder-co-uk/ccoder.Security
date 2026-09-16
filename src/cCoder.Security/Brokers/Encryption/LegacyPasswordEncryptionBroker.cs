// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Dependencies;

namespace cCoder.Security.Brokers.Encryption;

internal sealed class LegacyPasswordEncryptionBroker(
    AesCrypto<string> crypto)
    : ILegacyPasswordEncryptionBroker
{
    public string Decrypt(string encryptedPassword) =>
        crypto.Decrypt(source: encryptedPassword);
}