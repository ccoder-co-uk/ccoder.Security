// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Security.Brokers.Encryption;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SSOUserProcessingService(
    ISSOUserService ssoUserService,
    IPasswordEncryptionBroker encryptionBroker)
        : ISSOUserProcessingService
{
    public ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser user) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateSSOUserOnRegister(user: user);

            user.Id = GetNextAvailableUserId(user: user);
            user.PasswordHash = encryptionBroker.Encrypt(password: user.PasswordHash);

            return await ssoUserService.AddSSOUserAsync(item: user);
        });

    public ValueTask<SSOUser> InviteSSOUserAsync(SSOUser user) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateSSOUserOnInvite(user: user);

            user.Id = GetNextAvailableUserId(user: user);

            if (string.IsNullOrWhiteSpace(value: user.PasswordHash))
            {
                user.PasswordHash = Guid
                    .NewGuid()
                    .ToString(format: "N") + "Aa1!";
            }

            user.PasswordHash = encryptionBroker.Encrypt(password: user.PasswordHash);
            user.LockoutEnabled = true;

            return await ssoUserService.AddSSOUserAsync(item: user);
        });

    public ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser) =>
        TryCatch(operation: async () =>
        {
            ValidateSSOUserOnDelete(deletedSSOUser: deletedSSOUser);

            await ssoUserService.DeleteSSOUserAsync(item: deletedSSOUser);
        });

    public ValueTask<SSOUser> FindByUserAndPasswordAsync(
        string username,
        string password) =>
        TryCatch<SSOUser>(operation: async () =>
        {
                ValidateCredentialsOnFind(username: username, password: password);

                SSOUser user = FindSSOUserById(ssoUserId: username);

                if (user is null)
                {
                    throw new SecurityException("Access Denied!");
                }

                bool passwordMatches = encryptionBroker.EncryptedAndPlainTextAreEqual(
                    encrypted: user.PasswordHash,
                    plainText: password);

                if (!passwordMatches)
                {
                    user.AccessFailedCount++;

                    if (user.AccessFailedCount > 10)
                    {
                        user.LockoutEnabled = true;
                    }

                    await UpdateSSOUserCoreAsync(updatedSSOUser: user);
                    throw new SecurityException("Access Denied!");
                }

                if (user.AccessFailedCount > 0)
                {
                    user.AccessFailedCount = 0;
                    await UpdateSSOUserCoreAsync(updatedSSOUser: user);
                }

                if (user.LockoutEnabled)
                {
                    throw new SecurityException("Account locked!");
                }

                return user;
        });

    public SSOUser FindById(string ssoUserId) =>
        TryCatch(operation: () =>
        {
            ValidateSSOUserOnFind(ssoUserId: ssoUserId);

            return FindSSOUserById(ssoUserId: ssoUserId);
        });

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateSSOUsersOnGet(ignoreFilters: ignoreFilters);

            return ssoUserService.GetAllSSOUsers(ignoreFilters: ignoreFilters);
        });

    public ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateSSOUserOnUpdate(updatedSSOUser: updatedSSOUser);

            return await UpdateSSOUserCoreAsync(updatedSSOUser: updatedSSOUser);
        });

    public SSOUser Me() =>
        TryCatch(operation: () =>
        {
            ValidateSSOUserOnGetCurrent();

            return ssoUserService.Me();
        });

    public void ValidatePassword(string password) =>
        TryCatch(operation: () =>
        {
            ValidatePasswordInput(password: password);
            EnsurePasswordIsValid(password: password);
        });

    private SSOUser FindSSOUserById(string ssoUserId) =>
        ssoUserService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: user =>
                user.Id == ssoUserId ||
                user.Email == ssoUserId);

    private string GetNextAvailableUserId(SSOUser user)
    {
        string userId = user.Id;
        int attempts = 1;
        SSOUser existingUser = FindSSOUserById(ssoUserId: userId);

        while (existingUser is not null)
        {
            userId = user.Id + attempts;
            existingUser = FindSSOUserById(ssoUserId: userId);
            attempts++;
        }

        return userId;
    }

    private async ValueTask<SSOUser> UpdateSSOUserCoreAsync(SSOUser updatedSSOUser)
    {
        SSOUser storedUser = ssoUserService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: user => user.Id == updatedSSOUser.Id);

        bool passwordChanged =
            updatedSSOUser.PasswordHash is not null &&
            storedUser.PasswordHash != updatedSSOUser.PasswordHash;

        if (passwordChanged)
        {
            EnsurePasswordIsValid(password: updatedSSOUser.PasswordHash);

            updatedSSOUser.PasswordHash = encryptionBroker.Encrypt(
                password: updatedSSOUser.PasswordHash);
        }

        return await ssoUserService.UpdateSSOUserAsync(item: updatedSSOUser);
    }
}