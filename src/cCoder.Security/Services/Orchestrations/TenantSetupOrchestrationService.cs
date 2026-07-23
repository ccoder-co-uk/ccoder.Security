// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal class TenantSetupOrchestrationService(
    ITenantOrchestrationService tenantOrchestrationService,
    ISSOUserOrchestrationService ssoUserOrchestrationService) : ITenantSetupOrchestrationService
{
    public async ValueTask SetupAsync(SetupDetails setupDetails)
    {
        await tenantOrchestrationService.AddTenantAsync(item: setupDetails.Tenant);

        (SSOUser _, string confirmationToken) =
            await ssoUserOrchestrationService.Register(registerForm: MapRegisterUser(setupDetails));

        await ssoUserOrchestrationService.ConfirmRegistration(tokenId: confirmationToken);
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
}