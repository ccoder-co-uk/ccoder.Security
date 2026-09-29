// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------


using System;
using System.Linq;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class RequestService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateHeaderOnGet(string key) =>
        Validate(inputs: key);
}