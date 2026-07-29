// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace Security.AcceptanceTests.Tests.Models;

public class RegistrationResult
{
    public SSOUser User { get; set; }
    public string Token { get; set; }
}