using cCoder.Security.Data.Brokers.Encryption;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using System.Security;

namespace cCoder.Security.Services.Processing;

public partial class SSOUserProcessingService(
    ISSOUserService ssoUserService,
    IPasswordEncryptionBroker encryptionBroker) 
        : ISSOUserProcessingService
{
    public async ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser user)
    {
        ValidateSSOUser(user);

        user.Id = GetNextAvailableUserId(user);

        user.PasswordHash = encryptionBroker.Encrypt(user.PasswordHash);

        return await ssoUserService.AddSSOUserAsync(user);
    }

    public async ValueTask<SSOUser> InviteSSOUserAsync(SSOUser user)
    {
        ValidateSSOUser(user);

        user.Id = GetNextAvailableUserId(user);

        user.PasswordHash = encryptionBroker.Encrypt(user.PasswordHash);
        user.LockoutEnabled = true;

        return await ssoUserService.AddSSOUserAsync(user);
    }

    public async ValueTask DeleteSSOUserAsync(SSOUser item) =>
        await ssoUserService.DeleteSSOUserAsync(item);

    public async ValueTask<SSOUser> FindByUserAndPasswordAsync(string username, string password)
    {
        ValidateUsername(username);

        SSOUser user = FindById(username);

        if (user == null)
            throw new SecurityException("Access Denied!");

        if (!encryptionBroker.EncryptedAndPlainTextAreEqual(user.PasswordHash, password))
        {
            user.AccessFailedCount++;

            if (user.AccessFailedCount > 10)
                user.LockoutEnabled = true;

            await UpdateSSOUserAsync(user);
            throw new SecurityException("Access Denied!");
        }
        else
        {
            if(user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
                await UpdateSSOUserAsync(user);
            }
        }

        if (user.LockoutEnabled)
            throw new SecurityException("Account locked!");

        return user;
    }

    public SSOUser FindById(string id) =>
        ssoUserService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(u => u.Id == id || u.Email == id);

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) =>
        ssoUserService.GetAllSSOUsers(ignoreFilters);

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser user)
    {
        SSOUser dbUser = GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(u => u.Id == user.Id);

        if (user.PasswordHash != null && dbUser.PasswordHash != user.PasswordHash)
        {
            ValidatePassword(user.PasswordHash);
            user.PasswordHash = encryptionBroker.Encrypt(user.PasswordHash);
        }

        return await ssoUserService.UpdateSSOUserAsync(user);
    }

    public SSOUser Me() =>
        ssoUserService.Me();

    private string GetNextAvailableUserId(SSOUser user)
    {
        string userId = user.Id;
        int attempts = 1;

        SSOUser existing = FindById(userId);

        while (existing is not null)
        {
            userId = user.Id + attempts;

            existing = FindById(userId);
            attempts++;
        }

        return userId;
    }
}