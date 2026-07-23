// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Events;

public enum SecurityAccountEventKind
{
    RegistrationCreated,
    RegistrationConfirmed,
    InvitationCreated,
    InvitationAccepted,
    PasswordResetRequested
}