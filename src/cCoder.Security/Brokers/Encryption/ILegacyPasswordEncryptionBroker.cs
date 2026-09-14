// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.Encryption;

internal interface ILegacyPasswordEncryptionBroker : IUtilityBroker
{
    string Decrypt(string encryptedPassword);
}