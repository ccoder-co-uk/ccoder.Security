// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface ITokenManager
{
    ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse);
}