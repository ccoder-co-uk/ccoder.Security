// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace Security.AcceptanceTests.Exceptions;

public class InternalServerErrorException(string message) : Exception(message)
{
}