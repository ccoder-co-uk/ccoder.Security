// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Logging;

internal interface ILoggingBroker
{
    void LogException(Exception exception);
    void LogWarning(string message);
}