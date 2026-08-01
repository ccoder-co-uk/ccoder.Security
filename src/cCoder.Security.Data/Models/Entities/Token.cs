// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Models.Entities;

public class Token
{
    public string Id { get; set; }

    public int Reason { get; set; }

    public DateTimeOffset Expires { get; set; }

    public string UserName { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public string SecretHash { get; set; }

    public virtual SSOUser User { get; set; }
}

public enum TokenUse
{
    Auth,
    WorkflowExecution,
    PasswordReset,
    Confirmation,
    Invitation
}