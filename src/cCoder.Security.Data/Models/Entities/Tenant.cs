namespace cCoder.Security.Objects.Entities;

public class Tenant
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string CreatedBy { get; set; }

    public string LastUpdatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

    public ICollection<SSORole> Roles { get; set; }
    public ICollection<UserEvent> UserEvents { get; set; }
    public ICollection<TenantAnalysis> Analysis { get; set; }
}