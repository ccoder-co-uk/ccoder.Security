using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processing.Interfaces;

public interface ISSOUserRoleProcessingService
{
    IQueryable<SSOUserRole> GetAllSSOUserRoles();
    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item);
    ValueTask DeleteSSOUserRoleAsync(SSOUserRole item);
}