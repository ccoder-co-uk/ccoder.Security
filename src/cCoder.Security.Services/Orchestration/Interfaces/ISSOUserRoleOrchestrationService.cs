using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestration;
public interface ISSOUserRoleOrchestrationService
{
    IQueryable<SSOUserRole> GetAllSSOUserRoles();

    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole);

    ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole);
}