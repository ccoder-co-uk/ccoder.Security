// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects;

namespace cCoder.Security.Services.Aggregations.Interfaces;

internal interface ISSOAuthInfoAggregationService
{
    ValueTask<ISSOAuthInfo> GetSSOAuthInfoAsync();
}