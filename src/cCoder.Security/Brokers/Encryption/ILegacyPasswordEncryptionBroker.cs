// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Encryption;

internal interface ILegacyPasswordEncryptionBroker
{
    string Decrypt(string encryptedPassword);
}