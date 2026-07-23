// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Exceptions;

public sealed class SecurityManagementValidationException(Exception innerException)
    : Exception("Security management validation failed.", innerException)
{
}