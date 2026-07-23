// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Exceptions;

public sealed class SecurityOrchestrationServiceException(Exception innerException)
    : Exception("The Security orchestration service failed.", innerException)
{
}