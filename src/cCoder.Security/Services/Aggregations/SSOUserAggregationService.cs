// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Security.Models.Configurations;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class SSOUserAggregationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ILoggingProcessingService loggingProcessingService,
    ISSOAuthInfo authInfo)
        : ISSOUserAggregationService
{
    public SSOUser GetCurrentUser() =>
        TryCatch(operation: () =>
        {
            ValidateCurrentUserOnGet(authInfo: authInfo);

            return Sanitize(user: ssoUserProcessingService.Me());
        });

    public ValueTask<SSOUser> UpdateCurrentSSOUserAsync(SSOUser updatedSSOUser) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateCurrentSSOUserOnUpdate(
                updatedUser: updatedSSOUser,
                authInfo: authInfo);

            SSOUser currentUser = ssoUserProcessingService.Me();

            currentUser.DisplayName = updatedSSOUser.DisplayName;
            currentUser.Email = updatedSSOUser.Email;
            currentUser.PhoneNumber = updatedSSOUser.PhoneNumber;

            SSOUser result = await ssoUserProcessingService
                .UpdateSSOUserAsync(item: currentUser);

            return Sanitize(user: result);
        });

    public IQueryable<SSOUser> GetAllSSOUsers() =>
        TryCatch(operation: () =>
        {
            return ssoUserProcessingService.GetAllSSOUsers();
        });

    public ValueTask<SSOUser> UpdateSSOUserAsync(
        string username,
        SSOUser updatedSSOUser) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateSSOUserOnUpdate(
                username: username,
                updatedSSOUser: updatedSSOUser);

            SSOUser user = ssoUserProcessingService
                .GetAllSSOUsers()
                .FirstOrDefault(predicate: user => user.Id == username);

            if (user is null)
            {
                loggingProcessingService.LogWarning(
                    message: $"User not found: {username}");

                throw new SecurityException("Access Denied!");
            }

            user.DisplayName = updatedSSOUser.DisplayName;
            user.PhoneNumber = updatedSSOUser.PhoneNumber;
            user.Email = updatedSSOUser.Email;

            return await ssoUserProcessingService.UpdateSSOUserAsync(
                item: user);
        });

    public ValueTask DeleteSSOUserAsync(SSOUser deletedSSOUser) =>
        TryCatch(operation: async () =>
        {
            ValidateSSOUserOnDelete(deletedSSOUser: deletedSSOUser);

            await ssoUserProcessingService.DeleteSSOUserAsync(
                item: deletedSSOUser);
        });

    private static SSOUser Sanitize(SSOUser user) =>
        user is null
            ? null
            : new SSOUser
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed
            };
}