// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityManagementServiceException(Exception innerException)
    : Exception("The Security management service failed.", innerException)
{
}