// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface ISSOUserProcessingService
{
    ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser newSSOUser);

    ValueTask<SSOUser> InviteSSOUserAsync(SSOUser newSSOUser);

    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser);

    ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser);

    IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false);

    ValueTask<SSOUser> FindByUserAndPasswordAsync(string username, string password);

    SSOUser FindById(string id);

    SSOUser Me();

    void ValidatePassword(string password);
}