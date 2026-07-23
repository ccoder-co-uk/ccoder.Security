// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ITenantAnalysisService
{
    ValueTask<TenantAnalysis> AddTenantAnalaysisAsync(TenantAnalysis newTenantAnalysis);
    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis deletedTenantAnalysis);
    IQueryable<TenantAnalysis> GetAllTenantAnalysis();
    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis updatedTenantAnalysis);
}