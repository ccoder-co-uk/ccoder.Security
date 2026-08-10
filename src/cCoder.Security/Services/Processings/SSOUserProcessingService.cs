// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Security.Brokers.Encryption;
using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Models;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SSOUserProcessingService(
    ISSOUserService ssoUserService,
    IPasswordHashingBroker passwordHashingBroker,
    ISecurityDateTimeOffsetBroker dateTimeOffsetBroker,
    SecurityConfiguration securityConfiguration,
    ILegacyPasswordEncryptionBroker legacyEncryptionBroker = null)
        : ISSOUserProcessingService
{
    public ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser user) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateSSOUserOnRegister(user: user);

            user.Id = GetNextAvailableUserId(user: user);

            user.PasswordHash = passwordHashingBroker.HashPassword(
                password: user.PasswordHash);

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

            user.PasswordHash = passwordHashingBroker.HashPassword(
                password: user.PasswordHash);

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
                passwordHashingBroker.PerformDummyVerification(
                    providedPassword: password);

                throw new SecurityException("Access Denied!");
            }

            await EnsureAccountIsNotLockedAsync(user: user, password: password);

            PasswordVerificationOutcome passwordVerification = VerifyPassword(
                encryptedPassword: user.PasswordHash,
                plainTextPassword: password);

            if (passwordVerification == PasswordVerificationOutcome.Failed)
            {
                user.AccessFailedCount++;

                if (user.AccessFailedCount >= securityConfiguration.MaxFailedAccessAttempts)
                {
                    user.LockoutEnabled = true;

                    user.LockoutEndDateUtc = dateTimeOffsetBroker
                        .GetCurrentTime()
                        .UtcDateTime
                        .AddMinutes(
                            value: securityConfiguration.LockoutDurationMinutes);

                }

                await UpdateSSOUserCoreAsync(updatedSSOUser: user);
                throw new SecurityException("Access Denied!");
            }

            if (user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
                await UpdateSSOUserCoreAsync(updatedSSOUser: user);
            }

            if (passwordVerification == PasswordVerificationOutcome.SuccessRehashNeeded)
            {
                user.PasswordHash = passwordHashingBroker.HashPassword(
                    password: password);

                await ssoUserService.UpdateSSOUserAsync(item: user);
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

            updatedSSOUser.PasswordHash = passwordHashingBroker.HashPassword(
                password: updatedSSOUser.PasswordHash);
        }

        return await ssoUserService.UpdateSSOUserAsync(item: updatedSSOUser);
    }

    private PasswordVerificationOutcome VerifyPassword(
        string encryptedPassword,
        string plainTextPassword)
    {
        try
        {
            return passwordHashingBroker.VerifyHashedPassword(
                hashedPassword: encryptedPassword,
                providedPassword: plainTextPassword);
        }
        catch (FormatException) when (legacyEncryptionBroker is not null)
        {
            string legacyPassword = legacyEncryptionBroker.Decrypt(
                encryptedPassword: encryptedPassword);

            return legacyPassword == plainTextPassword
                ? PasswordVerificationOutcome.SuccessRehashNeeded
                : PasswordVerificationOutcome.Failed;
        }
    }

    private async ValueTask EnsureAccountIsNotLockedAsync(
        SSOUser user,
        string password)
    {
        if (!user.LockoutEnabled)
        {
            return;
        }

        DateTimeOffset currentTime = dateTimeOffsetBroker.GetCurrentTime();

        if (user.LockoutEndDateUtc is null
            || user.LockoutEndDateUtc > currentTime.UtcDateTime)
        {
            passwordHashingBroker.PerformDummyVerification(
                providedPassword: password);

            throw new SecurityException("Access Denied!");
        }

        user.LockoutEnabled = false;
        user.LockoutEndDateUtc = null;
        user.AccessFailedCount = 0;
        await UpdateSSOUserCoreAsync(updatedSSOUser: user);
    }
}