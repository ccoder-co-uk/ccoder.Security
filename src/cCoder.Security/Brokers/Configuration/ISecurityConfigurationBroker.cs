// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Configuration;

internal interface ISecurityConfigurationBroker
{
    string GetValue(string section, string key);
}