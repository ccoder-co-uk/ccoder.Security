// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Dependencies.Encryption;

internal interface ITokenGenerationDependency
{
    string GenerateSelector();

    string GenerateSecret();
}