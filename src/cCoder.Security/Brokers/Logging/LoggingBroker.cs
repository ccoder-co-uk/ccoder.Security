// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Logging;

internal sealed class LoggingBroker(ILogger<LoggingBroker> logger)
    : ILoggingBroker
{
    public void LogException(Exception exception) =>
        logger.LogError(
            exception: exception,
            message: exception.Message);

    public void LogWarning(string message) =>
        logger.LogWarning(message: message);
}