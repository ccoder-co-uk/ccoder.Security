// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Models;
using cCoder.Security.Dependencies;

namespace cCoder.Security.Services.Managements;

internal sealed partial class TenantSetupManagementService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateSetupDetailsOnSetup(SetupDetails setupDetails)
    {
        Validate(inputs: setupDetails);
        ArgumentNullException.ThrowIfNull(argument: setupDetails.Tenant);
        ArgumentNullException.ThrowIfNull(argument: setupDetails.User);

        if (string.IsNullOrWhiteSpace(value: setupDetails.Tenant.Id))
        {
            throw new ValidationException("Tenant ID is required.");
        }

        if (string.IsNullOrWhiteSpace(value: setupDetails.Tenant.Name))
        {
            throw new ValidationException("Tenant name is required.");
        }

        if (string.IsNullOrWhiteSpace(value: setupDetails.User.DisplayName))
        {
            throw new ValidationException("User display name is required.");
        }

        if (string.IsNullOrWhiteSpace(value: setupDetails.User.Email))
        {
            throw new ValidationException("User email is required.");
        }

        if (string.IsNullOrWhiteSpace(value: setupDetails.User.PasswordHash))
        {
            throw new ValidationException("User password is required.");
        }
    }
}