// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------
using cCoder.Security.Models.Configurations;
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
}