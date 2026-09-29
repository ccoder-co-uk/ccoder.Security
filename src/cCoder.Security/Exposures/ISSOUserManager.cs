// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface ISSOUserManager
{
    IQueryable<SSOUser> GetAllSSOUsers();

    ValueTask<SSOUser> UpdateSSOUserAsync(
        string username,
        SSOUser updatedSSOUser);

    ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser);
}