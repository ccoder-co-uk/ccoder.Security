// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Security.AcceptanceTests.Exceptions;

public class BadRequestException(string message) : Exception(message)
{
}