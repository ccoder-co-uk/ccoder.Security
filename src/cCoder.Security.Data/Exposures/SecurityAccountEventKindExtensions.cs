// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Events;

public static class SecurityAccountEventKindExtensions
{
    public static string ToEventName(
        this SecurityAccountEventKind securityAccountEventKind) =>
        securityAccountEventKind switch
        {
            SecurityAccountEventKind.RegistrationCreated =>
                "security_account_registration_created",
            SecurityAccountEventKind.RegistrationConfirmed =>
                "security_account_registration_confirmed",
            SecurityAccountEventKind.InvitationCreated =>
                "security_account_invitation_created",
            SecurityAccountEventKind.InvitationAccepted =>
                "security_account_invitation_accepted",
            SecurityAccountEventKind.PasswordResetRequested =>
                "security_account_password_reset_requested",
            SecurityAccountEventKind.TokenIssued =>
                "security_token_issued",
            SecurityAccountEventKind.LoginSucceeded =>
                "security_authentication_login_succeeded",
            SecurityAccountEventKind.LogoutSucceeded =>
                "security_authentication_logout_succeeded",
            SecurityAccountEventKind.AuthenticationFailed =>
                "security_authentication_failed",
            _ => throw new ArgumentOutOfRangeException(
                paramName: nameof(securityAccountEventKind))
        };
}