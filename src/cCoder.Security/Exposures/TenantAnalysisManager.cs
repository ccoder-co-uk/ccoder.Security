// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Processings;

namespace cCoder.Security.Exposures;

internal sealed class TenantAnalysisManager(
    ITenantAnalysisProcessingService tenantAnalysisProcessingService)
        : ITenantAnalysisManager
{
    public ValueTask<TenantAnalysis> AddTenantAnalysisAsync(
        TenantAnalysis newTenantAnalysis) =>
        tenantAnalysisProcessingService.AddTenantAnalysisAsync(
            item: newTenantAnalysis);

    public ValueTask DeleteTenantAnalysisAsync(
        TenantAnalysis deletedTenantAnalysis) =>
        tenantAnalysisProcessingService.DeleteTenantAnalysisAsync(
            item: deletedTenantAnalysis);

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis() =>
        tenantAnalysisProcessingService.GetAllTenantAnalysis();

    public ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(
        TenantAnalysis updatedTenantAnalysis) =>
        tenantAnalysisProcessingService.UpdateTenantAnalysisAsync(
            item: updatedTenantAnalysis);
}