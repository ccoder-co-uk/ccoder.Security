using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processing.Interfaces;

public interface ISSOUserRoleProcessingService
{
    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item);
    ValueTask DeleteSSOUserRoleAsync(SSOUserRole item);
}