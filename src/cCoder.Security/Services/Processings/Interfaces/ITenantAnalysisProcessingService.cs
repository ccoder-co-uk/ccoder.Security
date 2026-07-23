// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings;

public interface ITenantAnalysisProcessingService
{
    ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis item);

    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis item);

    IQueryable<TenantAnalysis> GetAllTenantAnalysis();

    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis item);
}