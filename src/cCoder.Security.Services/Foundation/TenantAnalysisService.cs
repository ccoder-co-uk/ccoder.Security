using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class TenantAnalysisService(ITenantAnalysisBroker broker) 
    : ITenantAnalysisService
{
    public async ValueTask<TenantAnalysis> AddTenantAnalaysisAsync(TenantAnalysis tenant)
    {
        tenant.CreatedOn = DateTimeOffset.UtcNow;
        return await broker.AddTenantAnalysisAsync(tenant);
    }

    public async ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenant)
        => await broker.DeleteTenantAnalysisAsync(tenant);

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis()
        => broker.GetAllTenantAnalysis();

    public async ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenant)
        => await broker.UpdateTenantAnalysisAsync(tenant);
}