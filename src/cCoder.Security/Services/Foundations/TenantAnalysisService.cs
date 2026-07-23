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
    public async ValueTask<TenantAnalysis> AddTenantAnalaysisAsync(TenantAnalysis tenant)
    {
        tenant.CreatedOn = DateTimeOffset.UtcNow;

        TenantAnalysis storageTenantAnalysis = new()
        {
            Id = tenant.Id,
            TenantId = tenant.TenantId,
            Key = tenant.Key,
            Name = tenant.Name,
            Value = tenant.Value,
            CreatedOn = tenant.CreatedOn
        };

        TenantAnalysis result = await broker.AddTenantAnalysisAsync(tenantAnalysis: storageTenantAnalysis);
        tenant.Id = result.Id;
        tenant.TenantId = result.TenantId;
        tenant.Key = result.Key;
        tenant.Name = result.Name;
        tenant.Value = result.Value;
        tenant.CreatedOn = result.CreatedOn;
        return tenant;
    }

    public ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenant)
        =>
        broker.DeleteTenantAnalysisAsync(tenantAnalysis: tenant);

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis()
        =>
        broker.GetAllTenantAnalysis();

    public async ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenant)
    {
        TenantAnalysis storageTenantAnalysis = new()
        {
            Id = tenant.Id,
            TenantId = tenant.TenantId,
            Key = tenant.Key,
            Name = tenant.Name,
            Value = tenant.Value,
            CreatedOn = tenant.CreatedOn
        };

        TenantAnalysis result = await broker.UpdateTenantAnalysisAsync(tenantAnalysis: storageTenantAnalysis);
        tenant.Id = result.Id;
        tenant.TenantId = result.TenantId;
        tenant.Key = result.Key;
        tenant.Name = result.Name;
        tenant.Value = result.Value;
        tenant.CreatedOn = result.CreatedOn;
        return tenant;
    }
}