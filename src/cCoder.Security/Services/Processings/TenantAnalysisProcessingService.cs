// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class TenantAnalysisProcessingService(ITenantAnalysisService tenantAnalysisService)
    : ITenantAnalysisProcessingService

{
    public ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis item) =>
        tenantAnalysisService.AddTenantAnalaysisAsync(tenant: item);

    public ValueTask DeleteTenantAnalysisAsync(TenantAnalysis item) =>
        tenantAnalysisService.DeleteTenantAnalysisAsync(tenant: item);

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis() =>
        tenantAnalysisService.GetAllTenantAnalysis();

    public ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis item) =>
        tenantAnalysisService.UpdateTenantAnalysisAsync(tenant: item);
}