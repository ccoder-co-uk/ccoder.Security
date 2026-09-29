// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Security.Brokers.DateTime;

internal interface ISecurityDateTimeOffsetBroker : IUtilityBroker
{
    DateTimeOffset GetCurrentTime();
}