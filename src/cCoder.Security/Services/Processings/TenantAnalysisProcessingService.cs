// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class TenantAnalysisProcessingService(ITenantAnalysisService tenantAnalysisService)
    : ITenantAnalysisProcessingService

{
    public ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis newTenantAnalysis) =>
        tenantAnalysisService.AddTenantAnalaysisAsync(newTenantAnalysis: newTenantAnalysis);

    public ValueTask DeleteTenantAnalysisAsync(TenantAnalysis deletedTenantAnalysis) =>
        tenantAnalysisService.DeleteTenantAnalysisAsync(deletedTenantAnalysis: deletedTenantAnalysis);

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis() =>
        tenantAnalysisService.GetAllTenantAnalysis();

    public ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis updatedTenantAnalysis) =>
        tenantAnalysisService.UpdateTenantAnalysisAsync(updatedTenantAnalysis: updatedTenantAnalysis);
}