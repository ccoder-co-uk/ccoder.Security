// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityOrchestrationDependencyException(Exception innerException)
    : Exception("A Security orchestration dependency failed.", innerException)
{
}