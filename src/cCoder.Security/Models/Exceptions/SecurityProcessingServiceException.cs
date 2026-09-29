// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

public sealed class SecurityProcessingServiceException(Exception innerException)
    : Exception("The Security processing service failed.", innerException)
{
}