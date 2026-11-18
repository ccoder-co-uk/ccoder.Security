// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using Microsoft.Extensions.Logging;

namespace cCoder.Security.Brokers.Logging;

internal sealed class LoggingBroker(ILogger<LoggingBroker> logger)
    : ILoggingBroker
{
    public void LogError(
        Exception exception,
        string message,
        params object[] args) =>
        logger.LogError(
            exception: exception,
            message: message,
            args: args);

    public void LogWarning(string message, params object[] args) =>
        logger.LogWarning(message: message, args: args);
}