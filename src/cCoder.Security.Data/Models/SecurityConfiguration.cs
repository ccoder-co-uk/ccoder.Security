// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Configurations;

namespace cCoder.Security.Models;

public class SecurityConfiguration
{
    public SecurityConfiguration()
    {
        ConnectionString = string.Empty;
        DecryptionKey = string.Empty;
        RootPath = "Api/Security";
        MaxFailedAccessAttempts = 10;
        LockoutDurationMinutes = 15;
        Argon = new ArgonConfiguration();
    }

    public string ConnectionString { get; set; }
    public string DecryptionKey { get; set; }
    public string RootPath { get; set; }
    public bool IsMigrating { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public int LockoutDurationMinutes { get; set; }
    public ArgonConfiguration Argon { get; set; }
}