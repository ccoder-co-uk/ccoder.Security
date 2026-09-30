// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ITenantAnalysisService
{
    ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis tenant);
    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenant);
    IQueryable<TenantAnalysis> GetAllTenantAnalyses();
    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenant);
}