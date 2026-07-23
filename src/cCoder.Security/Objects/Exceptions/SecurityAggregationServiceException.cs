// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Exceptions;

public sealed class SecurityAggregationServiceException(Exception innerException)
    : Exception("The Security aggregation service failed.", innerException)
{
}