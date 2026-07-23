// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ITenantAnalysisBroker
{
    ValueTask<TenantAnalysis> InsertTenantAnalysisAsync(TenantAnalysis newTenantAnalysis);
    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis deletedTenantAnalysis);
    IQueryable<TenantAnalysis> SelectAllTenantAnalysis();
    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis updatedTenantAnalysis);
}