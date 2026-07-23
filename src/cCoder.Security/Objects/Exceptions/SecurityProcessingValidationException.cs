// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Exceptions;

public sealed class SecurityProcessingValidationException(Exception innerException)
    : Exception("Security processing validation failed.", innerException)
{
}