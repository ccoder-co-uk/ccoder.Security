// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Objects.Events;

public sealed class SecurityAccountEventRequest
{
    public SecurityAccountEventKind Kind { get; set; }

    public SSOUser User { get; set; }

    public RegisterUser RegisterForm { get; set; }

    public string Token { get; set; }
}