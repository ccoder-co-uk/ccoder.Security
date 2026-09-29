// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityOrchestrationValidationException(Exception innerException)
    : Exception("Security orchestration validation failed.", innerException)
{
}