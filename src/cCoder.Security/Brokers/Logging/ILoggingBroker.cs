// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Brokers.Logging;

internal interface ILoggingBroker
{
    void LogException(Exception exception);
    void LogWarning(string message);
}