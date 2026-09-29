// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityManagementValidationException(Exception innerException)
    : Exception("Security management validation failed.", innerException)
{
}