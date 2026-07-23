// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface ISSOUserProcessingService
{
    ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser item);

    ValueTask<SSOUser> InviteSSOUserAsync(SSOUser user);

    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser item);

    ValueTask DeleteSSOUserAsync(SSOUser item);

    IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false);

    ValueTask<SSOUser> FindByUserAndPasswordAsync(string username, string password);

    SSOUser FindById(string ssoUserId);

    SSOUser Me();

    void ValidatePassword(string password);
}