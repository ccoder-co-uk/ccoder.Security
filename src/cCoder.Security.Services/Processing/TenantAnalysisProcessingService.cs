using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Processing;

public class TenantAnalysisProcessingService(ITenantAnalysisService tenantAnalysisService) 
    : ITenantAnalysisProcessingService

{
    public ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis item) =>
        tenantAnalysisService.AddTenantAnalaysisAsync(item);

    public ValueTask DeleteTenantAnalysisAsync(TenantAnalysis item) =>
        tenantAnalysisService.DeleteTenantAnalysisAsync(item);

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis() =>
        tenantAnalysisService.GetAllTenantAnalysis();

    public ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis item) =>
        tenantAnalysisService.UpdateTenantAnalysisAsync(item);
}