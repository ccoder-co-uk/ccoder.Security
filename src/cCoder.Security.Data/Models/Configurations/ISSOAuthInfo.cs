// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Configurations;

public interface ISSOAuthInfo
{
    public bool AuthenticationFailed { get; set; }

    public string SSOUserId { get; set; }
}