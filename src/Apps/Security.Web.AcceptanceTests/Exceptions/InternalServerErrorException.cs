// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Security.AcceptanceTests.Exceptions;

public class InternalServerErrorException(string message) : Exception(message)
{
}