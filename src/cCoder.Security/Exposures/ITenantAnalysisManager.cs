// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface ITenantAnalysisManager
{
    ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis item);

    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis item);

    IQueryable<TenantAnalysis> GetAllTenantAnalysis();

    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis item);
}