// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class ConfirmForgotPasswordController(
    IAuthenticationManager authenticationAggregationService)
        : Controller
{
    [HttpPost("ConfirmForgotPassword")]
    public async ValueTask<IActionResult> PostConfirmForgotPassword(
        [FromBody] ConfirmForgotPasswordRequest newConfirmForgotPasswordRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            await authenticationAggregationService.ConfirmForgotPasswordAsync(
                tokenId: newConfirmForgotPasswordRequest.Token,
                userId: newConfirmForgotPasswordRequest.UserId,
                newPassword: newConfirmForgotPasswordRequest.NewPassword,
                confirmNewPassword: newConfirmForgotPasswordRequest.ConfirmPassword);

            return Ok();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The password reset request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }
}