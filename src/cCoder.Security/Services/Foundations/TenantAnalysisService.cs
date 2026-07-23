// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class TenantAnalysisService(ITenantAnalysisBroker broker)
    : ITenantAnalysisService
{
    public ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis newTenantAnalysis) =>
        TryCatch<TenantAnalysis>(operation: async () =>
        {
            ValidateTenantAnalysisOnAdd(newTenantAnalysis: newTenantAnalysis);
            newTenantAnalysis.CreatedOn = DateTimeOffset.UtcNow;

            TenantAnalysis storageTenantAnalysis = new()
            {
                Id = newTenantAnalysis.Id,
                TenantId = newTenantAnalysis.TenantId,
                Key = newTenantAnalysis.Key,
                Name = newTenantAnalysis.Name,
                Value = newTenantAnalysis.Value,
                CreatedOn = newTenantAnalysis.CreatedOn
            };

            TenantAnalysis result = await broker.InsertTenantAnalysisAsync(
                tenantAnalysis: storageTenantAnalysis);

            CopyTenantAnalysis(
                sourceTenantAnalysis: result,
                targetTenantAnalysis: newTenantAnalysis);

            return newTenantAnalysis;
        });

    public ValueTask DeleteTenantAnalysisAsync(TenantAnalysis deletedTenantAnalysis) =>
        TryCatch(operation: async () =>
        {
            ValidateTenantAnalysisOnDelete(deletedTenantAnalysis: deletedTenantAnalysis);

            await broker.DeleteTenantAnalysisAsync(tenantAnalysis: deletedTenantAnalysis);
        });

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis() =>
        TryCatch(operation: () => broker.SelectAllTenantAnalysis());

    public ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis updatedTenantAnalysis) =>
        TryCatch<TenantAnalysis>(operation: async () =>
        {
            ValidateTenantAnalysisOnUpdate(updatedTenantAnalysis: updatedTenantAnalysis);

            TenantAnalysis storageTenantAnalysis = new()
            {
                Id = updatedTenantAnalysis.Id,
                TenantId = updatedTenantAnalysis.TenantId,
                Key = updatedTenantAnalysis.Key,
                Name = updatedTenantAnalysis.Name,
                Value = updatedTenantAnalysis.Value,
                CreatedOn = updatedTenantAnalysis.CreatedOn
            };

            TenantAnalysis result = await broker.UpdateTenantAnalysisAsync(
                tenantAnalysis: storageTenantAnalysis);

            CopyTenantAnalysis(
                sourceTenantAnalysis: result,
                targetTenantAnalysis: updatedTenantAnalysis);

            return updatedTenantAnalysis;
        });

    private static void CopyTenantAnalysis(
        TenantAnalysis sourceTenantAnalysis,
        TenantAnalysis targetTenantAnalysis)
    {
        targetTenantAnalysis.Id = sourceTenantAnalysis.Id;
        targetTenantAnalysis.TenantId = sourceTenantAnalysis.TenantId;
        targetTenantAnalysis.Key = sourceTenantAnalysis.Key;
        targetTenantAnalysis.Name = sourceTenantAnalysis.Name;
        targetTenantAnalysis.Value = sourceTenantAnalysis.Value;
        targetTenantAnalysis.CreatedOn = sourceTenantAnalysis.CreatedOn;
    }
}