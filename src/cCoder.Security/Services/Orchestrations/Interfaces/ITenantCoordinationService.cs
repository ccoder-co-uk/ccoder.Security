using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;
internal interface ITenantCoordinationService
{
    ValueTask DeleteTenantAsync(Tenant item);
}

