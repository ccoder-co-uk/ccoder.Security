// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Exposures;

public interface ITokenManager
{
    ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse);
}