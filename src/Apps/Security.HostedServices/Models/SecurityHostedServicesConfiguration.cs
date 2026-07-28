// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects;

namespace Security.HostedServices.Models;

public sealed class SecurityHostedServicesConfiguration
{
    public SecurityHostedServicesConfiguration() =>
        Security = new SecurityConfiguration();

    public SecurityConfiguration Security { get; set; }
}