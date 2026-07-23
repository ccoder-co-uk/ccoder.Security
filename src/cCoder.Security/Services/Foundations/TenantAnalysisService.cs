// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal class TenantAnalysisService(ITenantAnalysisBroker broker)
    : ITenantAnalysisService
{
    public async ValueTask<TenantAnalysis> AddTenantAnalaysisAsync(TenantAnalysis newTenantAnalysis)
    {
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

        TenantAnalysis result = await broker.InsertTenantAnalysisAsync(tenantAnalysis: storageTenantAnalysis);
        newTenantAnalysis.Id = result.Id;
        newTenantAnalysis.TenantId = result.TenantId;
        newTenantAnalysis.Key = result.Key;
        newTenantAnalysis.Name = result.Name;
        newTenantAnalysis.Value = result.Value;
        newTenantAnalysis.CreatedOn = result.CreatedOn;
        return newTenantAnalysis;
    }

    public ValueTask DeleteTenantAnalysisAsync(TenantAnalysis deletedTenantAnalysis)
        =>
        broker.DeleteTenantAnalysisAsync(tenantAnalysis: deletedTenantAnalysis);

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis()
        =>
        broker.SelectAllTenantAnalysis();

    public async ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis updatedTenantAnalysis)
    {
        TenantAnalysis storageTenantAnalysis = new()
        {
            Id = updatedTenantAnalysis.Id,
            TenantId = updatedTenantAnalysis.TenantId,
            Key = updatedTenantAnalysis.Key,
            Name = updatedTenantAnalysis.Name,
            Value = updatedTenantAnalysis.Value,
            CreatedOn = updatedTenantAnalysis.CreatedOn
        };

        TenantAnalysis result = await broker.UpdateTenantAnalysisAsync(tenantAnalysis: storageTenantAnalysis);
        updatedTenantAnalysis.Id = result.Id;
        updatedTenantAnalysis.TenantId = result.TenantId;
        updatedTenantAnalysis.Key = result.Key;
        updatedTenantAnalysis.Name = result.Name;
        updatedTenantAnalysis.Value = result.Value;
        updatedTenantAnalysis.CreatedOn = result.CreatedOn;
        return updatedTenantAnalysis;
    }
}