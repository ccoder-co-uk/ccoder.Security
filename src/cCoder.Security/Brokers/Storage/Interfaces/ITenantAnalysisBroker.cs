// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ITenantAnalysisBroker
{
    ValueTask<TenantAnalysis> InsertTenantAnalysisAsync(TenantAnalysis tenantAnalysis);
    ValueTask DeleteTenantAnalysisAsync(TenantAnalysis tenantAnalysis);
    IQueryable<TenantAnalysis> SelectAllTenantAnalysis();
    ValueTask<TenantAnalysis> UpdateTenantAnalysisAsync(TenantAnalysis tenantAnalysis);
}