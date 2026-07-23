// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class SSOAuthInfoAggregationService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSSOAuthInfoOnGet() =>
        Validate(inputs: []);
}