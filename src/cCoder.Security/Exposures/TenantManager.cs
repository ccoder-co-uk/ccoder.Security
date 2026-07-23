// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal class TenantManager(IRegistrationAggregationService registrationAggregationService)
    : ITenantManager
{
    public ValueTask SetupAsync(SetupDetails setupDetails)
    {
        RegisterUser newRegisterUser = new()
        {
            DisplayName = setupDetails.User.DisplayName,
            Email = setupDetails.User.Email,
            Password = setupDetails.User.PasswordHash,
            PhoneNumber = setupDetails.User.PhoneNumber,
            Culture = string.Empty,
            AppId = 0,
            TenantId = setupDetails.Tenant.Id,
            Tenant = setupDetails.Tenant,
            User = setupDetails.User
        };

        return registrationAggregationService.SetupRegisterUserAsync(
            newRegisterUser: newRegisterUser);
    }
}