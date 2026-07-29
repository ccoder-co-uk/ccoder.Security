// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Models;

public sealed class AuthorizationContext
{
    public SSOUser CurrentUser { get; set; }

    public IEnumerable<SSOPrivilege> Privileges { get; set; }
}