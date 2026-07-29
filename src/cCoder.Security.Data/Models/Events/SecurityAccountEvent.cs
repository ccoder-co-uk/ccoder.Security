// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Models.Events;

public class SecurityAccountEvent
{
    public SecurityAccountEventKind Kind { get; set; }

    public SSOUser User { get; set; }

    public Tenant Tenant { get; set; }

    public string RequestDomain { get; set; }

    public string Token { get; set; }

    public string Culture { get; set; }
}