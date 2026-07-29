// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SessionProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateStringOnGet(string key) =>
        Validate(inputs: key);

    private static void ValidateUserOnGet() =>
        Validate(inputs: []);

    private static void ValidateStringOnSet(string key, string value)
    {
        Validate(inputs: key);

        if (value is not null)
        {
            Validate(inputs: value);
        }
    }

    private static void ValidateSSOUserOnSet(SSOUser user)
    {
        if (user is not null)
        {
            Validate(inputs: user);
        }
        else
        {
            Validate(inputs: []);
        }
    }

    private static void ValidateSessionOnRemove(string key) =>
        Validate(inputs: key);

    private static void ValidateSessionOnClear() =>
        Validate(inputs: []);
}