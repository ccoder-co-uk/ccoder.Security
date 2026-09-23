// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Security.Brokers.Encryption.Interfaces;

internal interface ITokenGenerationBroker : IUtilityBroker
{
    string GenerateSelector();

    string GenerateSecret();

    string Combine(string selector, string secret);

    string[] Split(string token);
}