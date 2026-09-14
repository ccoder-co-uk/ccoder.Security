// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.DateTime;

internal interface ISecurityDateTimeOffsetBroker : IUtilityBroker
{
    DateTimeOffset GetCurrentTime();
}