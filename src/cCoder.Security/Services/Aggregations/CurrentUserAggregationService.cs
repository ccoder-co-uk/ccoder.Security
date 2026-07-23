// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class CurrentUserAggregationService(
    ISSOUserProcessingService ssoUserProcessingService)
        : ICurrentUserAggregationService
{
    public SSOUser GetCurrentUser() =>
        TryCatch(operation: () =>
        {
            ValidateCurrentUserOnGet();

            return Sanitize(user: ssoUserProcessingService.Me());
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