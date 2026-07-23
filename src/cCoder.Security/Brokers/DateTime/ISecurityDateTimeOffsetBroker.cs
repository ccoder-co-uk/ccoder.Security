// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.DateTime;

internal interface ISecurityDateTimeOffsetBroker
{
    DateTimeOffset GetCurrentTime();
}