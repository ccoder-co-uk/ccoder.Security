// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Models;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;

namespace cCoder.Security.Services.Processings.Events;

internal class TenantSetupEventProcessingService(
    ITenantSetupOrchestrationService tenantSetupOrchestrationService) : ITenantSetupEventProcessingService
{
    public ValueTask SetupAsync(SetupDetails setupDetails)
    {
        Validate(setupDetails: setupDetails);
        NormalizeSetupDetails(setupDetails: setupDetails);

        return tenantSetupOrchestrationService.SetupAsync(setupDetails: setupDetails);
    }

    private static void NormalizeSetupDetails(SetupDetails setupDetails)
    {
        setupDetails.User.Id = ResolveUserId(user: setupDetails.User);

        setupDetails.Tenant.CreatedBy = string.IsNullOrWhiteSpace(value: setupDetails.Tenant.CreatedBy)
            ? setupDetails.User.Id
            : setupDetails.Tenant.CreatedBy;

        setupDetails.Tenant.LastUpdatedBy = string.IsNullOrWhiteSpace(value: setupDetails.Tenant.LastUpdatedBy)
            ? setupDetails.User.Id
            : setupDetails.Tenant.LastUpdatedBy;

        setupDetails.Tenant.Description = string.IsNullOrWhiteSpace(value: setupDetails.Tenant.Description)
            ? $"{setupDetails.Tenant.Name} tenant"
            : setupDetails.Tenant.Description;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (setupDetails.Tenant.CreatedOn == default)
        { setupDetails.Tenant.CreatedOn = now; }
        if (setupDetails.Tenant.LastUpdated == default)
        { setupDetails.Tenant.LastUpdated = now; }
    }

    private static string ResolveUserId(SSOUser user)
    {
        if (!string.IsNullOrWhiteSpace(value: user.Id))
        { return user.Id; }

        if (string.IsNullOrWhiteSpace(value: user.Email) || !user.Email.Contains(value: '@'))
        { throw new ValidationException("A valid user email is required for bootstrap setup."); }

        return user.Email.Split(separator: "@")[0];
    }

    private static void Validate(SetupDetails setupDetails)
    {
        ArgumentNullException.ThrowIfNull(argument: setupDetails);
        ArgumentNullException.ThrowIfNull(argument: setupDetails.Tenant);
        ArgumentNullException.ThrowIfNull(argument: setupDetails.User);

        if (string.IsNullOrWhiteSpace(value: setupDetails.Tenant.Id))
        { throw new ValidationException("Tenant ID is required."); }

        if (string.IsNullOrWhiteSpace(value: setupDetails.Tenant.Name))
        { throw new ValidationException("Tenant name is required."); }

        if (string.IsNullOrWhiteSpace(value: setupDetails.User.DisplayName))
        { throw new ValidationException("User display name is required."); }

        if (string.IsNullOrWhiteSpace(value: setupDetails.User.Email))
        { throw new ValidationException("User email is required."); }

        if (string.IsNullOrWhiteSpace(value: setupDetails.User.PasswordHash))
        { throw new ValidationException("User password is required."); }
    }
}