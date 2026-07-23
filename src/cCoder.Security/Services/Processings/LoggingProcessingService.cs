// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class LoggingProcessingService(
    ILoggingService loggingService)
        : ILoggingProcessingService
{
    public void LogWarning(string message) =>
        TryCatch(operation: () =>
        {
            ValidateWarningOnLog(message: message);

            loggingService.LogWarning(message: message);
        });
}