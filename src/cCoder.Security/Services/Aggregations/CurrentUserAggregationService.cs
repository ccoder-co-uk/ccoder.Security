// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Configurations;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class CurrentUserAggregationService(
    ISSOUserProcessingService ssoUserProcessingService,
    ISSOAuthInfo authInfo)
        : ICurrentUserAggregationService
{
    public SSOUser GetCurrentUser() =>
        TryCatch(operation: () =>
        {
            ValidateCurrentUserOnGet(authInfo: authInfo);

            return Sanitize(user: ssoUserProcessingService.Me());
        });

    public ValueTask<SSOUser> UpdateCurrentSSOUserAsync(SSOUser updatedUser) =>
        TryCatch<SSOUser>(operation: async () =>
        {
            ValidateCurrentUserOnUpdate(
                updatedUser: updatedUser,
                authInfo: authInfo);

            SSOUser currentUser = ssoUserProcessingService.Me();

            currentUser.DisplayName = updatedUser.DisplayName;
            currentUser.Email = updatedUser.Email;
            currentUser.PhoneNumber = updatedUser.PhoneNumber;

            SSOUser result = await ssoUserProcessingService
                .UpdateSSOUserAsync(item: currentUser);

            return Sanitize(user: result);
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