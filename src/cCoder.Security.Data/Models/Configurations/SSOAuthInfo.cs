// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Configurations;

public class SSOAuthInfo : ISSOAuthInfo
{
    public bool AuthenticationFailed { get; set; }

    public string SSOUserId { get; set; }
}