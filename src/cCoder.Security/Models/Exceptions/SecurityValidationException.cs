// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityValidationException(Exception innerException)
    : Exception("Security validation failed.", innerException)
{
}