// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class TenantAnalysisProcessingService(
    ITenantAnalysisService tenantAnalysisService)
    : ITenantAnalysisProcessingService
{
    public ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis newTenantAnalysis) =>
        TryCatch<TenantAnalysis>(operation: async () =>
        {
            ValidateTenantAnalysisOnAdd(newTenantAnalysis: newTenantAnalysis);

            return await tenantAnalysisService.AddTenantAnalysisAsync(
                tenant: newTenantAnalysis);
        });

    public ValueTask DeleteTenantAnalysisAsync(TenantAnalysis deletedTenantAnalysis) =>
        TryCatch(operation: async () =>
        {
            ValidateTenantAnalysisOnDelete(deletedTenantAnalysis: deletedTenantAnalysis);

            await tenantAnalysisService.DeleteTenantAnalysisAsync(
                tenant: deletedTenantAnalysis);
        });

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis() =>
        TryCatch(operation: () =>
        {
            ValidateTenantAnalysisOnGet();

            return tenantAnalysisService.GetAllTenantAnalysis();
        });

    public ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis updatedTenantAnalysis) =>
        TryCatch<TenantAnalysis>(operation: async () =>
        {
            ValidateTenantAnalysisOnUpdate(updatedTenantAnalysis: updatedTenantAnalysis);

            return await tenantAnalysisService.UpdateTenantAnalysisAsync(
                tenant: updatedTenantAnalysis);
        });
}