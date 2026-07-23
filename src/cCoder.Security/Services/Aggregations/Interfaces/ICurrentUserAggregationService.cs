// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Aggregations.Interfaces;

public interface ICurrentUserAggregationService
{
    SSOUser GetCurrentUser();
}