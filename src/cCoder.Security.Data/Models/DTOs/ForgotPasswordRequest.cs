// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.DTOs;

public class ForgotPasswordRequest
{
    public string Email { get; set; }
    public int AppId { get; set; }
}