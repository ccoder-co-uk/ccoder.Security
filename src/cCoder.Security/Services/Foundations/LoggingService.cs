// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Logging;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class LoggingService(ILoggingBroker loggingBroker)
    : ILoggingService
{
    public void LogWarning(string message) =>
        TryCatch(operation: () =>
        {
            ValidateWarningOnLog(message: message);

            loggingBroker.LogWarning(message: message);
        });
}