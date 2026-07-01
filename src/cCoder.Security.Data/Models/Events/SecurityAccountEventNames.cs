namespace cCoder.Security.Objects.Events;

public static class SecurityAccountEventNames
{
    public const string RegistrationCreated = "security_account_registration_created";
    public const string RegistrationConfirmed = "security_account_registration_confirmed";
    public const string InvitationCreated = "security_account_invitation_created";
    public const string InvitationAccepted = "security_account_invitation_accepted";
    public const string PasswordResetRequested = "security_account_password_reset_requested";
}
