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
    public async ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser user)
    {
        ValidateSSOUser(user: user);

        user.Id = GetNextAvailableUserId(user: user);

        user.PasswordHash = encryptionBroker.Encrypt(password: user.PasswordHash);

        return await ssoUserService.AddSSOUserAsync(item: user);
    }

    public async ValueTask<SSOUser> InviteSSOUserAsync(SSOUser user)
    {
        ValidateSSOUser(user: user, validatePassword: false);

        user.Id = GetNextAvailableUserId(user: user);

        if (string.IsNullOrWhiteSpace(value: user.PasswordHash))
        { user.PasswordHash = Guid.NewGuid().ToString(format: "N") + "Aa1!"; }

        user.PasswordHash = encryptionBroker.Encrypt(password: user.PasswordHash);
        user.LockoutEnabled = true;

        return await ssoUserService.AddSSOUserAsync(item: user);
    }

    public ValueTask DeleteSSOUserAsync(SSOUser item) =>
        ssoUserService.DeleteSSOUserAsync(item: item);

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

            await UpdateSSOUserAsync(user: user);
            throw new SecurityException("Access Denied!");
        }
        else
        {
            if (user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
                await UpdateSSOUserAsync(user: user);
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

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser user)
    {
        SSOUser dbUser = GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(predicate: u => u.Id == user.Id);

        if (user.PasswordHash != null && dbUser.PasswordHash != user.PasswordHash)
        {
            ValidatePassword(password: user.PasswordHash);
            user.PasswordHash = encryptionBroker.Encrypt(password: user.PasswordHash);
        }

        return await ssoUserService.UpdateSSOUserAsync(item: user);
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