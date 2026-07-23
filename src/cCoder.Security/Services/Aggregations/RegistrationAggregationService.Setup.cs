// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class RegistrationAggregationService
{
    private async ValueTask AddBootstrapTenantAsync(
        RegisterUser newRegisterUser)
    {
        Tenant existingTenant = tenantProcessingService
            .GetAllTenants()
            .FirstOrDefault(predicate: tenant =>
                tenant.Id == newRegisterUser.Tenant.Id);

        if (existingTenant is not null)
        {
            throw new ValidationException(
                $"Tenant '{newRegisterUser.Tenant.Id}' already exists.");
        }

        await tenantProcessingService.AddTenantAsync(
            item: newRegisterUser.Tenant);

        await roleProcessingService.AddSSORoleAsync(
            item: new SSORole
            {
                UsersArePortalAdmins = true,
                Name = "Administrators",
                Description = "Bootstrap tenant administrators",
                Privs = string.Empty,
                TenantId = newRegisterUser.Tenant.Id
            });
    }

    private static void NormalizeRegisterUser(RegisterUser registerUser)
    {
        registerUser.User.Id = ResolveUserId(user: registerUser.User);

        registerUser.Tenant.CreatedBy = string.IsNullOrWhiteSpace(
            value: registerUser.Tenant.CreatedBy)
                ? registerUser.User.Id
                : registerUser.Tenant.CreatedBy;

        registerUser.Tenant.LastUpdatedBy = string.IsNullOrWhiteSpace(
            value: registerUser.Tenant.LastUpdatedBy)
                ? registerUser.User.Id
                : registerUser.Tenant.LastUpdatedBy;

        registerUser.Tenant.Description = string.IsNullOrWhiteSpace(
            value: registerUser.Tenant.Description)
                ? $"{registerUser.Tenant.Name} tenant"
                : registerUser.Tenant.Description;

        DateTimeOffset now = DateTimeOffset.UtcNow;

        if (registerUser.Tenant.CreatedOn == default)
        {
            registerUser.Tenant.CreatedOn = now;
        }

        if (registerUser.Tenant.LastUpdated == default)
        {
            registerUser.Tenant.LastUpdated = now;
        }
    }

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