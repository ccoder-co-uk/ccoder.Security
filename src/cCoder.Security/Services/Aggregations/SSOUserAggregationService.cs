// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class SSOUserAggregationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ILoggingProcessingService loggingProcessingService)
        : ISSOUserAggregationService
{
    public IQueryable<SSOUser> GetAllSSOUsers() =>
        TryCatch(operation: () =>
        {
            ValidateSSOUsersOnGet();

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
}