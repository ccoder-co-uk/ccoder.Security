// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;

namespace cCoder.Security.Services.Aggregations.Interfaces;

internal interface ISSOAuthInfoAggregationService
{
    ValueTask<ISSOAuthInfo> GetSSOAuthInfoAsync();
}