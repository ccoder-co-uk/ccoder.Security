// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class ForgotPasswordController(
    IAuthenticationAggregationService authenticationAggregationService,
    ILoggingBroker loggingBroker)
        : Controller, ICompositionExposure
{
    [HttpPost("ForgotPassword")]
    public async ValueTask<IActionResult> PostForgotPassword(
        [FromBody] ForgotPasswordRequest newForgotPasswordRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            await authenticationAggregationService.ForgotPasswordAsync(
                email: newForgotPasswordRequest.Email);
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The password reset request is invalid.");
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

        return Ok();
    }
}