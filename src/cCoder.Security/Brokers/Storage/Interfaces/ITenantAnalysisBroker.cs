using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;
internal interface ITenantAnalysisBroker
{
    ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis tenantAnalysis);
    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenantAnalysis);
    IQueryable<TenantAnalysis> GetAllTenantAnalysis();
    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenantAnalysis);
}

