// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.Configuration;

internal interface ISecurityConfigurationBroker : IUtilityBroker
{
    string GetValue(string section, string key);
}