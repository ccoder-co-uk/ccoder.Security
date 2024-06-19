using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestration;
public interface ISSORoleOrchestrationService
{
    ValueTask<SSORole> AddSSORoleAsync(SSORole item);
    ValueTask DeleteSSORoleAsync(SSORole item);
    IQueryable<SSORole> GetAllSSORoles();
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole item);
}