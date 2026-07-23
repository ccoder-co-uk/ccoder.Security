// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Exceptions;

public sealed class SecurityProcessingDependencyException(Exception innerException)
    : Exception("A Security processing dependency failed.", innerException)
{
}