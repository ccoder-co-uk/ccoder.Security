// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models;

public class SecurityConfiguration
{
    public SecurityConfiguration()
    {
        ConnectionString = string.Empty;
        DecryptionKey = string.Empty;
        RootPath = "Api/Security";
    }

    public string ConnectionString { get; set; }
    public string DecryptionKey { get; set; }
    public string RootPath { get; set; }
    public bool IsMigrating { get; set; }
}