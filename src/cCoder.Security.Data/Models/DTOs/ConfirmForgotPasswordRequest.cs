// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.DTOs;

public class ConfirmForgotPasswordRequest
{
    public string ConfirmPassword { get; set; }
    public string NewPassword { get; set; }
    public int SourceAppId { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
}