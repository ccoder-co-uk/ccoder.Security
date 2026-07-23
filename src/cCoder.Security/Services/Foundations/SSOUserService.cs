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

        SSOUser result = await ssoUserBroker.InsertSSOUserAsync(user: storageUser);
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

    public ValueTask DeleteSSOUserAsync(SSOUser item) =>
        ssoUserBroker.DeleteSSOUserAsync(SSOUser: item);

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser item)
    {
        SSOUser storageUser = new()
        {
            Id = item.Id,
            DisplayName = item.DisplayName,
            Email = item.Email,
            PhoneNumber = item.PhoneNumber,
            PasswordHash = item.PasswordHash,
            AccessFailedCount = item.AccessFailedCount,
            EmailConfirmed = item.EmailConfirmed,
            LockoutEnabled = item.LockoutEnabled,
            LockoutEndDateUtc = item.LockoutEndDateUtc,
            PhoneNumberConfirmed = item.PhoneNumberConfirmed
        };

        SSOUser result = await ssoUserBroker.UpdateSSOUserAsync(user: storageUser);
        item.Id = result.Id;
        item.DisplayName = result.DisplayName;
        item.Email = result.Email;
        item.PhoneNumber = result.PhoneNumber;
        item.PasswordHash = result.PasswordHash;
        item.AccessFailedCount = result.AccessFailedCount;
        item.EmailConfirmed = result.EmailConfirmed;
        item.LockoutEnabled = result.LockoutEnabled;
        item.LockoutEndDateUtc = result.LockoutEndDateUtc;
        item.PhoneNumberConfirmed = result.PhoneNumberConfirmed;
        return item;
    }

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) =>
        ssoUserBroker.SelectAllSSOUsers(ignoreFilters: ignoreFilters);

    public SSOUser Me() =>
        ssoUserBroker.SelectCurrentSSOUser();
}