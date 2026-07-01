namespace cCoder.Security.Objects.Entities;

public class Token
{
    public string Id { get; set; }

    public int Reason { get; set; }

    public DateTimeOffset Expires { get; set; }

    public string UserName { get; set; }

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
