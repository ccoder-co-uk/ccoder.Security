// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Processings;

public interface ITenantAnalysisProcessingService
{
    ValueTask<TenantAnalysis> AddTenantAnalysisAsync(TenantAnalysis item);

    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis item);

    IQueryable<TenantAnalysis> GetAllTenantAnalysis();

    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis item);
}