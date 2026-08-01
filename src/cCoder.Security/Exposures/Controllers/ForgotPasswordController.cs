// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class ForgotPasswordController(
    IAuthenticationManager authenticationAggregationService)
        : Controller
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
        catch (SecurityAggregationValidationException)
        {
            return Ok();
        }
        catch (SecurityAggregationDependencyException)
        {
            return Ok();
        }
        catch (SecurityAggregationServiceException exception)
            when (ContainsSecurityException(exception: exception))
        {
            return Ok();
        }
        catch
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }

        return Ok();
    }

    private static bool ContainsSecurityException(Exception exception) =>
        exception is SecurityException
        || exception.InnerException is not null
            && ContainsSecurityException(exception: exception.InnerException);
}