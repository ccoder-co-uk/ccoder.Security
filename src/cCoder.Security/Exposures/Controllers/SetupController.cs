// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Setup")]
public sealed class SetupController(
    IRegistrationAggregationService registrationAggregationService)
    : Controller
{
    [HttpPost]
    public async ValueTask<IActionResult> PostSetup(
        [FromBody] SetupDetails newSetupDetails)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            RegisterUser newRegisterUser = new()
            {
                DisplayName = newSetupDetails.User.DisplayName,
                Email = newSetupDetails.User.Email,
                Password = newSetupDetails.User.PasswordHash,
                PhoneNumber = newSetupDetails.User.PhoneNumber,
                Culture = string.Empty,
                AppId = 0,
                TenantId = newSetupDetails.Tenant.Id,
                Tenant = newSetupDetails.Tenant,
                User = newSetupDetails.User
            };

            await registrationAggregationService.SetupRegisterUserAsync(
                newRegisterUser: newRegisterUser);

            return Ok();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The setup request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
        }
        catch (SecurityAggregationServiceException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }
}