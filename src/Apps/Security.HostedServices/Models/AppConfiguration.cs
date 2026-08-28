// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;

namespace Security.HostedServices.Models;

public sealed class AppConfiguration
{
    public AppConfiguration()
    {
        Security = new SecurityConfiguration();
        SecurityData = new SecurityDataConfiguration();
    }

    public SecurityConfiguration Security { get; set; }

    public SecurityDataConfiguration SecurityData { get; set; }
}