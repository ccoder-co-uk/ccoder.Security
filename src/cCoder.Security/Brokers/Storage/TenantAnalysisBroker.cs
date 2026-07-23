// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cCoder.Security.Brokers.Storage;

internal class TenantAnalysisBroker(
    ISecurityDbContextFactory contextFactory)
        : ITenantAnalysisBroker
{
    public async ValueTask<TenantAnalysis> InsertTenantAnalysisAsync(TenantAnalysis tenantAnalysis)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantAnalysis> entityEntry =
            await context.TenantAnalysis.AddAsync(entity: tenantAnalysis);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenantAnalysis)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantAnalysis> entityEntry =
            context.TenantAnalysis.Update(entity: tenantAnalysis);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenantAnalysis)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantAnalysis> entityEntry =
            context.TenantAnalysis.Remove(entity: tenantAnalysis);

        await context.SaveChangesAsync();
    }

    public IQueryable<TenantAnalysis> SelectAllTenantAnalysis()
    {
        var context = contextFactory.CreateDbContext();
        return context.TenantAnalysis;
    }
}