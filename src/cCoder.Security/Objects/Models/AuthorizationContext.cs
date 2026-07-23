// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Objects.Models;

public sealed class AuthorizationContext
{
    public SSOUser CurrentUser { get; set; }

    public IEnumerable<SSOPrivilege> Privileges { get; set; }
}