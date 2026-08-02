// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Events;

public enum SecurityAccountEventKind
{
    RegistrationCreated,
    RegistrationConfirmed,
    InvitationCreated,
    InvitationAccepted,
    PasswordResetRequested,
    TokenIssued,
    LoginSucceeded,
    LogoutSucceeded,
    AuthenticationFailed
}