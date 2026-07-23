// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal class SSOUserService(ISSOUserBroker ssoUserBroker)
    : ISSOUserService
{
    public async ValueTask<SSOUser> AddSSOUserAsync(SSOUser newUser)
    {
        SSOUser storageUser = new()
        {
            Id = newUser.Id,
            DisplayName = newUser.DisplayName,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber,
            PasswordHash = newUser.PasswordHash,
            AccessFailedCount = newUser.AccessFailedCount,
            EmailConfirmed = newUser.EmailConfirmed,
            LockoutEnabled = newUser.LockoutEnabled,
            LockoutEndDateUtc = newUser.LockoutEndDateUtc,
            PhoneNumberConfirmed = newUser.PhoneNumberConfirmed
        };

        SSOUser result = await ssoUserBroker.InsertSSOUserAsync(newSSOUser: storageUser);
        newUser.Id = result.Id;
        newUser.DisplayName = result.DisplayName;
        newUser.Email = result.Email;
        newUser.PhoneNumber = result.PhoneNumber;
        newUser.PasswordHash = result.PasswordHash;
        newUser.AccessFailedCount = result.AccessFailedCount;
        newUser.EmailConfirmed = result.EmailConfirmed;
        newUser.LockoutEnabled = result.LockoutEnabled;
        newUser.LockoutEndDateUtc = result.LockoutEndDateUtc;
        newUser.PhoneNumberConfirmed = result.PhoneNumberConfirmed;
        return newUser;
    }

    public ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser) =>
        ssoUserBroker.DeleteSSOUserAsync(deletedSSOUser: deletedSSOUser);

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser)
    {
        SSOUser storageUser = new()
        {
            Id = updatedSSOUser.Id,
            DisplayName = updatedSSOUser.DisplayName,
            Email = updatedSSOUser.Email,
            PhoneNumber = updatedSSOUser.PhoneNumber,
            PasswordHash = updatedSSOUser.PasswordHash,
            AccessFailedCount = updatedSSOUser.AccessFailedCount,
            EmailConfirmed = updatedSSOUser.EmailConfirmed,
            LockoutEnabled = updatedSSOUser.LockoutEnabled,
            LockoutEndDateUtc = updatedSSOUser.LockoutEndDateUtc,
            PhoneNumberConfirmed = updatedSSOUser.PhoneNumberConfirmed
        };

        SSOUser result = await ssoUserBroker.UpdateSSOUserAsync(updatedSSOUser: storageUser);
        updatedSSOUser.Id = result.Id;
        updatedSSOUser.DisplayName = result.DisplayName;
        updatedSSOUser.Email = result.Email;
        updatedSSOUser.PhoneNumber = result.PhoneNumber;
        updatedSSOUser.PasswordHash = result.PasswordHash;
        updatedSSOUser.AccessFailedCount = result.AccessFailedCount;
        updatedSSOUser.EmailConfirmed = result.EmailConfirmed;
        updatedSSOUser.LockoutEnabled = result.LockoutEnabled;
        updatedSSOUser.LockoutEndDateUtc = result.LockoutEndDateUtc;
        updatedSSOUser.PhoneNumberConfirmed = result.PhoneNumberConfirmed;
        return updatedSSOUser;
    }

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) =>
        ssoUserBroker.SelectAllSSOUsers(ignoreFilters: ignoreFilters);

    public SSOUser Me() =>
        ssoUserBroker.SelectCurrentSSOUser();
}