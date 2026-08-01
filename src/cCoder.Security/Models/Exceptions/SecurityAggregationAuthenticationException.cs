// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityAggregationAuthenticationException(
    Exception innerException)
        : Exception(
            message: "A security authentication error occurred.",
            innerException: innerException)
{ }