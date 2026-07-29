// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityOrchestrationValidationException(Exception innerException)
    : Exception("Security orchestration validation failed.", innerException)
{
}