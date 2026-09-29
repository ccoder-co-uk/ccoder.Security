// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityAggregationValidationException(Exception innerException)
    : Exception("Security aggregation validation failed.", innerException)
{
}