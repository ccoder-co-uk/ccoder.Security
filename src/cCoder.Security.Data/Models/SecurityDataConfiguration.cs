// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models;

public sealed class SecurityDataConfiguration
{
    public SecurityDataConfiguration()
    {
        ConnectionString = string.Empty;
        AdminConnectionString = string.Empty;
    }

    public string ConnectionString { get; set; }

    public string AdminConnectionString { get; set; }
}