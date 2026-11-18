// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Security.Brokers.Logging;

public interface ILoggingBroker : IUtilityBroker
{
    void LogWarning(string message, params object[] args);

    void LogError(Exception exception, string message, params object[] args);
}