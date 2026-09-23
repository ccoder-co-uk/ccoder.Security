// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Security.Brokers.Encryption;

internal interface ILegacyPasswordEncryptionBroker : IUtilityBroker
{
    string Decrypt(string encryptedPassword);
}