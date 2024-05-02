using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundation.Interfaces;
public interface ITenantAnalysisService
{
    ValueTask<TenantAnalysis> AddTenantAnalaysisAsync(TenantAnalysis tenant);
    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenant);
    IQueryable<TenantAnalysis> GetAllTenantAnalysis();
    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenant);
}