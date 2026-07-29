// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityServiceException(Exception innerException)
    : Exception("The Security service failed.", innerException)
{
}