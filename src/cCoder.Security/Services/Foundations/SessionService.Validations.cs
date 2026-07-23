// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SessionService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSessionOnSet(string key, string value)
    {
        Validate(inputs: key);

        if (value is not null)
        {
            Validate(inputs: value);
        }
    }

    private static void ValidateStringOnGet(string key) =>
        Validate(inputs: key);

    private static void ValidateUserOnGet() =>
        Validate(inputs: []);

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
}