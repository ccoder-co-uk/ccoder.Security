// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Models;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Managements.Interfaces;

namespace cCoder.Security.Services.Managements;

internal sealed partial class TenantSetupManagementService(
    ITenantAggregationService tenantAggregationService,
    ISSOUserAggregationService ssoUserAggregationService)
        : ITenantSetupManagementService
{
    public ValueTask SetupDetailsAsync(SetupDetails setupDetails) =>
        TryCatch(operation: async () =>
        {
            ValidateSetupDetailsOnSetup(setupDetails: setupDetails);
            NormalizeSetupDetails(setupDetails: setupDetails);

            await tenantAggregationService.AddTenantAsync(
                item: setupDetails.Tenant);

            (SSOUser _, string confirmationToken) =
                await ssoUserAggregationService.RegisterUserAsync(
                    registerForm: MapRegisterUser(setupDetails: setupDetails));

            await ssoUserAggregationService.ConfirmRegistration(
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