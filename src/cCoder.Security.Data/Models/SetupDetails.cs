// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Models;

public class SetupDetails
{
    public Tenant Tenant { get; set; }

    public SSOUser User { get; set; }
}