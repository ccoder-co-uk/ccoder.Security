using System.Text.Json.Serialization;

namespace cCoder.Security.Objects.Entities;

public class TenantSecret
{
    public Guid Id { get; set; }

    public string TenantId { get; set; }

    public string Key { get; set; }

    [JsonIgnore]
    public string Value { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public string CreatedBy { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public string UpdatedBy { get; set; }

    public Tenant Tenant { get; set; }
}
