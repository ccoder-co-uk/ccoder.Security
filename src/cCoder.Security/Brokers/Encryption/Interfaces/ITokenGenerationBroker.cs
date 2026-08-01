// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Encryption.Interfaces;

internal interface ITokenGenerationBroker
{
    string GenerateSelector();

    string GenerateSecret();

    string Combine(string selector, string secret);

    string[] Split(string token);
}