// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSOUserService(ISSOUserBroker ssoUserBroker)
    : ISSOUserService
{
    public ValueTask<SSOUser> AddSSOUserAsync(SSOUser newUser) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateSSOUserOnAdd(newUser: newUser);

            SSOUser storageUser = CreateStorageSSOUser(ssoUser: newUser);
            SSOUser result = await ssoUserBroker.InsertSSOUserAsync(user: storageUser);
            CopySSOUser(sourceSSOUser: result, targetSSOUser: newUser);

            return newUser;
        });

    public ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser) =>
        TryCatch(operation: async () =>
        {
            ValidateSSOUserOnDelete(deletedSSOUser: deletedSSOUser);

            await ssoUserBroker.DeleteSSOUserAsync(SSOUser: deletedSSOUser);
        });

    public ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateSSOUserOnUpdate(updatedSSOUser: updatedSSOUser);

            SSOUser storageUser = CreateStorageSSOUser(ssoUser: updatedSSOUser);
            SSOUser result = await ssoUserBroker.UpdateSSOUserAsync(user: storageUser);
            CopySSOUser(sourceSSOUser: result, targetSSOUser: updatedSSOUser);

            return updatedSSOUser;
        });

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllSSOUsersOnGet(ignoreFilters: ignoreFilters);

            return ignoreFilters
                ? ssoUserBroker.SelectAllSSOUsersIgnoringFilters()
                : ssoUserBroker.SelectAllSSOUsers();
        });

    public SSOUser Me() =>
        TryCatch(operation: () => ssoUserBroker.SelectCurrentSSOUser());

    private static SSOUser CreateStorageSSOUser(SSOUser ssoUser) =>
        new()
        {
            Id = ssoUser.Id,
            DisplayName = ssoUser.DisplayName,
            Email = ssoUser.Email,
            PhoneNumber = ssoUser.PhoneNumber,
            PasswordHash = ssoUser.PasswordHash,
            AccessFailedCount = ssoUser.AccessFailedCount,
            EmailConfirmed = ssoUser.EmailConfirmed,
            LockoutEnabled = ssoUser.LockoutEnabled,
            LockoutEndDateUtc = ssoUser.LockoutEndDateUtc,
            PhoneNumberConfirmed = ssoUser.PhoneNumberConfirmed
        };

    private static void CopySSOUser(SSOUser sourceSSOUser, SSOUser targetSSOUser)
    {
        targetSSOUser.Id = sourceSSOUser.Id;
        targetSSOUser.DisplayName = sourceSSOUser.DisplayName;
        targetSSOUser.Email = sourceSSOUser.Email;
        targetSSOUser.PhoneNumber = sourceSSOUser.PhoneNumber;
        targetSSOUser.PasswordHash = sourceSSOUser.PasswordHash;
        targetSSOUser.AccessFailedCount = sourceSSOUser.AccessFailedCount;
        targetSSOUser.EmailConfirmed = sourceSSOUser.EmailConfirmed;
        targetSSOUser.LockoutEnabled = sourceSSOUser.LockoutEnabled;
        targetSSOUser.LockoutEndDateUtc = sourceSSOUser.LockoutEndDateUtc;
        targetSSOUser.PhoneNumberConfirmed = sourceSSOUser.PhoneNumberConfirmed;
    }
}