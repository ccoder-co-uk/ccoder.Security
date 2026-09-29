// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace Security.AcceptanceTests.Exceptions;

public class BadRequestException(string message) : Exception(message)
{
}