using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace cCoder.Security.Data.Brokers.Storage;

public class TenantAnalysisBroker(
    ISecurityDbContextFactory contextFactory) 
        : ITenantAnalysisBroker
{
    public async ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis tenantAnalysis)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantAnalysis> entityEntry = 
            await context.TenantAnalysis.AddAsync(tenantAnalysis);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenantAnalysis)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantAnalysis> entityEntry = 
            context.TenantAnalysis.Update(tenantAnalysis);

        await context.SaveChangesAsync();

        return entityEntry.Entity;
    }

    public async ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenantAnalysis)
    {
        using var context = contextFactory.CreateDbContext();

        EntityEntry<TenantAnalysis> entityEntry = 
            context.TenantAnalysis.Remove(tenantAnalysis);

        await context.SaveChangesAsync();
    }

    public IQueryable<TenantAnalysis> GetAllTenantAnalysis()
    {
        var context = contextFactory.CreateDbContext();
        return context.TenantAnalysis;
    }
}