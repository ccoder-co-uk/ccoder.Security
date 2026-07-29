// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityManagementDependencyException(Exception innerException)
    : Exception("A Security management dependency failed.", innerException)
{
}