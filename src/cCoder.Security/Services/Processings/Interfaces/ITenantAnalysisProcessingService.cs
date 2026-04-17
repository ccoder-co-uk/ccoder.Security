using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings;
internal interface ITenantAnalysisProcessingService
{
    ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis item);
    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis item);
    IQueryable<TenantAnalysis> GetAllTenantAnalysis();
    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis item);
}

