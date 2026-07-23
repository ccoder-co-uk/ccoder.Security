// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Encryption;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using System.Security;

namespace cCoder.Security.Services.Processings;

internal partial class SSOUserProcessingService(
    ISSOUserService ssoUserService,
    IPasswordEncryptionBroker encryptionBroker)
        : ISSOUserProcessingService
{
    public async ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser newSSOUser)
    {
        ValidateSSOUser(user: newSSOUser);

        newSSOUser.Id = GetNextAvailableUserId(user: newSSOUser);

        newSSOUser.PasswordHash = encryptionBroker.Encrypt(password: newSSOUser.PasswordHash);

        return await ssoUserService.AddSSOUserAsync(newSSOUser: newSSOUser);
    }

    public async ValueTask<SSOUser> InviteSSOUserAsync(SSOUser newSSOUser)
    {
        ValidateSSOUser(user: newSSOUser, validatePassword: false);

        newSSOUser.Id = GetNextAvailableUserId(user: newSSOUser);

        if (string.IsNullOrWhiteSpace(value: newSSOUser.PasswordHash))
        {
            newSSOUser.PasswordHash = Guid
                .NewGuid()
                .ToString(format: "N") + "Aa1!";
        }

        newSSOUser.PasswordHash = encryptionBroker.Encrypt(password: newSSOUser.PasswordHash);
        newSSOUser.LockoutEnabled = true;

        return await ssoUserService.AddSSOUserAsync(newSSOUser: newSSOUser);
    }

    public ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser) =>
        ssoUserService.DeleteSSOUserAsync(deletedSSOUser: deletedSSOUser);

    public async ValueTask<SSOUser> FindByUserAndPasswordAsync(string username, string password)
    {
        ValidateUsername(username: username);

        SSOUser user = FindById(id: username);

        if (user == null)
        { throw new SecurityException("Access Denied!"); }

        if (!encryptionBroker.EncryptedAndPlainTextAreEqual(encrypted: user.PasswordHash, plainText: password))
        {
            user.AccessFailedCount++;

            if (user.AccessFailedCount > 10)
            { user.LockoutEnabled = true; }

            await UpdateSSOUserAsync(updatedSSOUser: user);
            throw new SecurityException("Access Denied!");
        }
        else
        {
            if (user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
                await UpdateSSOUserAsync(updatedSSOUser: user);
            }
        }

        if (user.LockoutEnabled)
        { throw new SecurityException("Account locked!"); }

        return user;
    }

    public SSOUser FindById(string id) =>
        ssoUserService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: u => u.Id == id || u.Email == id);

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) =>
        ssoUserService.GetAllSSOUsers(ignoreFilters: ignoreFilters);

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser updatedSSOUser)
    {
        SSOUser dbUser = GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: u => u.Id == updatedSSOUser.Id);

        if (updatedSSOUser.PasswordHash != null && dbUser.PasswordHash != updatedSSOUser.PasswordHash)
        {
            ValidatePassword(password: updatedSSOUser.PasswordHash);
            updatedSSOUser.PasswordHash = encryptionBroker.Encrypt(password: updatedSSOUser.PasswordHash);
        }

        return await ssoUserService.UpdateSSOUserAsync(updatedSSOUser: updatedSSOUser);
    }

    public SSOUser Me() =>
        ssoUserService.Me();

    private string GetNextAvailableUserId(SSOUser user)
    {
        string userId = user.Id;
        int attempts = 1;

        SSOUser existing = FindById(id: userId);

        while (existing is not null)
        {
            userId = user.Id + attempts;

            existing = FindById(id: userId);
            attempts++;
        }

        return userId;
    }
}