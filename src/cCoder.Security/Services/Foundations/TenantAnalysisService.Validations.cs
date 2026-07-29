// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class TenantAnalysisService
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

    private static void ValidateTenantAnalysisOnUpdate(TenantAnalysis updatedTenantAnalysis) =>
        Validate(inputs: updatedTenantAnalysis);

    private static void ValidateTenantAnalysisOnDelete(TenantAnalysis deletedTenantAnalysis) =>
        Validate(inputs: deletedTenantAnalysis);
}