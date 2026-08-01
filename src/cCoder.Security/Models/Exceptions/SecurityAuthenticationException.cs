// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Exceptions;

internal sealed class SecurityAuthenticationException(string message)
    : Exception(message)
{ }