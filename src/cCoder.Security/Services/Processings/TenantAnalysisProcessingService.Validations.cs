// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Processings;

internal sealed partial class TenantAnalysisProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateTenantAnalysisOnAdd(TenantAnalysis newTenantAnalysis) =>
        Validate(inputs: newTenantAnalysis);

    private static void ValidateTenantAnalysisOnDelete(TenantAnalysis deletedTenantAnalysis) =>
        Validate(inputs: deletedTenantAnalysis);

    private static void ValidateTenantAnalysisOnUpdate(TenantAnalysis updatedTenantAnalysis) =>
        Validate(inputs: updatedTenantAnalysis);
}