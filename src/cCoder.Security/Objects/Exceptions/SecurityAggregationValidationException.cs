// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Exceptions;

public sealed class SecurityAggregationValidationException(Exception innerException)
    : Exception("Security aggregation validation failed.", innerException)
{
}