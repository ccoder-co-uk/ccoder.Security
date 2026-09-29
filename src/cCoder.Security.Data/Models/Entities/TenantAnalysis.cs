// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Entities;

public class TenantAnalysis
{
    public Guid Id { get; set; }

    public string TenantId { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }

    public string Value { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public virtual Tenant Tenant { get; set; }
}