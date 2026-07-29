// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityDependencyException(Exception innerException)
    : Exception("A Security dependency failed.", innerException)
{
}