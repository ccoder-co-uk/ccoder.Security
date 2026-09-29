// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityProcessingDependencyException(Exception innerException)
    : Exception("A Security processing dependency failed.", innerException)
{
}