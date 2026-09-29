// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityOrchestrationServiceException(Exception innerException)
    : Exception("The Security orchestration service failed.", innerException)
{
}