// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Data.Models;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Setup")]
public sealed class SetupController(
    IRegistrationAggregationService registrationAggregationService,
    ILoggingBroker loggingBroker)
    : Controller, ICompositionExposure
{
    [HttpPost]
    public async ValueTask<IActionResult> PostSetup(
        [FromBody] SetupDetails newSetupDetails)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

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

            IActionResult successResponse = Ok();

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return successResponse;
        }
        catch (ArgumentException)
        {
            loggingBroker.LogWarning(message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            loggingBroker.LogWarning(message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The setup request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
        }
        catch (SecurityAggregationServiceException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }
}