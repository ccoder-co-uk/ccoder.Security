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
        Validate(setupDetails);
        NormalizeSetupDetails(setupDetails);

        return tenantSetupOrchestrationService.SetupAsync(setupDetails);
    }

    private static void NormalizeSetupDetails(SetupDetails setupDetails)
    {
        setupDetails.User.Id = ResolveUserId(setupDetails.User);
        setupDetails.Tenant.CreatedBy = string.IsNullOrWhiteSpace(setupDetails.Tenant.CreatedBy)
            ? setupDetails.User.Id
            : setupDetails.Tenant.CreatedBy;
        setupDetails.Tenant.LastUpdatedBy = string.IsNullOrWhiteSpace(setupDetails.Tenant.LastUpdatedBy)
            ? setupDetails.User.Id
            : setupDetails.Tenant.LastUpdatedBy;
        setupDetails.Tenant.Description = string.IsNullOrWhiteSpace(setupDetails.Tenant.Description)
            ? $"{setupDetails.Tenant.Name} tenant"
            : setupDetails.Tenant.Description;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (setupDetails.Tenant.CreatedOn == default)
            setupDetails.Tenant.CreatedOn = now;
        if (setupDetails.Tenant.LastUpdated == default)
            setupDetails.Tenant.LastUpdated = now;
    }

    private static string ResolveUserId(SSOUser user)
    {
        if (!string.IsNullOrWhiteSpace(user.Id))
            return user.Id;

        if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains('@'))
            throw new ValidationException("A valid user email is required for bootstrap setup.");

        return user.Email.Split("@")[0];
    }

    private static void Validate(SetupDetails setupDetails)
    {
        ArgumentNullException.ThrowIfNull(setupDetails);
        ArgumentNullException.ThrowIfNull(setupDetails.Tenant);
        ArgumentNullException.ThrowIfNull(setupDetails.User);

        if (string.IsNullOrWhiteSpace(setupDetails.Tenant.Id))
            throw new ValidationException("Tenant ID is required.");

        if (string.IsNullOrWhiteSpace(setupDetails.Tenant.Name))
            throw new ValidationException("Tenant name is required.");

        if (string.IsNullOrWhiteSpace(setupDetails.User.DisplayName))
            throw new ValidationException("User display name is required.");

        if (string.IsNullOrWhiteSpace(setupDetails.User.Email))
            throw new ValidationException("User email is required.");

        if (string.IsNullOrWhiteSpace(setupDetails.User.PasswordHash))
            throw new ValidationException("User password is required.");
    }
}

