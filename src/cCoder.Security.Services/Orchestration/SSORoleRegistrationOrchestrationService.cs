using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Orchestration;

public class SSORoleOrchestrationService(
    ISSORoleProcessingService ssoRoleProcessingService) 
        : ISSORoleOrchestrationService
{
    public IQueryable<SSORole> GetAllSSORoles() =>
        ssoRoleProcessingService.GetAllSSORoles();

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole item)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole item)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteSSORoleAsync(SSORole item)
    {
        throw new NotImplementedException();
    }
}