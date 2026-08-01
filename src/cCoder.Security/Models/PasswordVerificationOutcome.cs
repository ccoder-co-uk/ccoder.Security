// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models;

internal enum PasswordVerificationOutcome
{
    Failed,
    Success,
    SuccessRehashNeeded
}