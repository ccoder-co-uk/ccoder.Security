// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface ISecurityCurrentUserManager
{
    SSOUser GetCurrentUser();

    ValueTask<SSOUser> UpdateCurrentSSOUserAsync(SSOUser updatedSSOUser);
}