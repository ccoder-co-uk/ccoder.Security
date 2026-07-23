// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Entities;

public class SSOPrivilege
{
    public string Id { get; set; }

    public string Type { get; set; }

    public string Operation { get; set; }

    public string Description { get; set; }

    public bool PortalAdminsOnly { get; set; }
}