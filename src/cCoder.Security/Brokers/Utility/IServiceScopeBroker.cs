// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Security.Brokers.Utility;

internal interface IServiceScopeBroker : IUtilityBroker
{
    ValueTask ExecuteInScopeAsync<TService>(
        Func<TService, ValueTask> operation)
        where TService : notnull;
}