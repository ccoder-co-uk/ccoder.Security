using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Objects.Events;

public class SecurityAccountEvent
{
    public SecurityAccountEventKind Kind { get; set; }

    public SSOUser User { get; set; }

    public Tenant Tenant { get; set; }

    public string RequestDomain { get; set; }

    public string Token { get; set; }

    public string Culture { get; set; }
}
