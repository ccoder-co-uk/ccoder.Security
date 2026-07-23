// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Extensions.Configuration;

namespace cCoder.Security.Brokers.Configuration;

internal sealed class SecurityConfigurationBroker(IConfiguration configuration)
    : ISecurityConfigurationBroker
{
    public string GetValue(string section, string key) =>
        configuration.GetSection(key: section)[key];
}