// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------
using cCoder.Security.Models.Configurations;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class CurrentUserAggregationService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateCurrentUserOnGet(ISSOAuthInfo authInfo)
    {
        Validate(inputs: [authInfo]);

        if (authInfo.AuthenticationFailed)
        {
            throw new SecurityAuthenticationException(
                message: "The supplied authentication credentials are invalid.");
        }
    }

    private static void ValidateCurrentUserOnUpdate(
        SSOUser updatedUser,
        ISSOAuthInfo authInfo)
    {
        Validate(inputs: [updatedUser, authInfo]);

        if (authInfo.AuthenticationFailed)
        {
            throw new SecurityAuthenticationException(
                message: "The supplied authentication credentials are invalid.");
        }

        if (string.IsNullOrWhiteSpace(value: updatedUser.DisplayName)
            || string.IsNullOrWhiteSpace(value: updatedUser.Email))
        {
            throw new ArgumentException(
                message: "Display name and email are required.");
        }
    }
}