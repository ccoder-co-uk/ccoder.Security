// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;

namespace Security.Web.Models;

public sealed class SecurityWebConfiguration
{
    public SecurityWebConfiguration() =>
        Security = new SecurityConfiguration();

    public SecurityConfiguration Security { get; set; }
}