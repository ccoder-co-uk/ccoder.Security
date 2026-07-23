// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Exceptions;

public sealed class SecurityAggregationDependencyException(Exception innerException)
    : Exception("A Security aggregation dependency failed.", innerException)
{
}