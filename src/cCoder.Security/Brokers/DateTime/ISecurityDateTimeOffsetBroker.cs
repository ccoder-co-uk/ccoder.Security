// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Security.Brokers.DateTime;

internal interface ISecurityDateTimeOffsetBroker : IUtilityBroker
{
    DateTimeOffset GetCurrentTime();
}