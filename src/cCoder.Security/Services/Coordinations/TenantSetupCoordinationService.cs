// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Models;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Coordinations.Interfaces;
using cCoder.Security.Services.Orchestrations.Interfaces;

namespace cCoder.Security.Services.Coordinations;

internal sealed partial class TenantSetupCoordinationService(
    ITenantOrchestrationService tenantOrchestrationService,
    ISSOUserOrchestrationService ssoUserOrchestrationService)
        : ITenantSetupCoordinationService
{
    public ValueTask SetupDetailsAsync(SetupDetails setupDetails) =>
        TryCatch(operation: async () =>
        {
            ValidateSetupDetailsOnSetup(setupDetails: setupDetails);
            NormalizeSetupDetails(setupDetails: setupDetails);

            await tenantOrchestrationService.AddTenantAsync(
                item: setupDetails.Tenant);

            (SSOUser _, string confirmationToken) =
                await ssoUserOrchestrationService.RegisterUserAsync(
                    registerForm: MapRegisterUser(setupDetails: setupDetails));

            await ssoUserOrchestrationService.ConfirmRegistration(
                tokenId: confirmationToken);
        });

    private static void NormalizeSetupDetails(SetupDetails setupDetails)
    {
        setupDetails.User.Id = ResolveUserId(user: setupDetails.User);

        setupDetails.Tenant.CreatedBy = string.IsNullOrWhiteSpace(
            value: setupDetails.Tenant.CreatedBy)
                ? setupDetails.User.Id
                : setupDetails.Tenant.CreatedBy;

        setupDetails.Tenant.LastUpdatedBy = string.IsNullOrWhiteSpace(
            value: setupDetails.Tenant.LastUpdatedBy)
                ? setupDetails.User.Id
                : setupDetails.Tenant.LastUpdatedBy;

        setupDetails.Tenant.Description = string.IsNullOrWhiteSpace(
            value: setupDetails.Tenant.Description)
                ? $"{setupDetails.Tenant.Name} tenant"
                : setupDetails.Tenant.Description;

        DateTimeOffset now = DateTimeOffset.UtcNow;

        if (setupDetails.Tenant.CreatedOn == default)
        {
            setupDetails.Tenant.CreatedOn = now;
        }

        if (setupDetails.Tenant.LastUpdated == default)
        {
            setupDetails.Tenant.LastUpdated = now;
        }
    }

    private static RegisterUser MapRegisterUser(SetupDetails setupDetails) =>
        new()
        {
            DisplayName = setupDetails.User.DisplayName,
            Email = setupDetails.User.Email,
            Password = setupDetails.User.PasswordHash,
            PhoneNumber = setupDetails.User.PhoneNumber,
            Culture = string.Empty,
            AppId = 0,
            TenantId = setupDetails.Tenant.Id
        };

    private static string ResolveUserId(SSOUser user)
    {
        if (!string.IsNullOrWhiteSpace(value: user.Id))
        {
            return user.Id;
        }

        bool emailIsInvalid =
            string.IsNullOrWhiteSpace(value: user.Email) ||
            !user.Email.Contains(value: '@');

        if (emailIsInvalid)
        {
            throw new ValidationException(
                "A valid user email is required for bootstrap setup.");
        }

        return user.Email.Split(separator: "@")[0];
    }
}