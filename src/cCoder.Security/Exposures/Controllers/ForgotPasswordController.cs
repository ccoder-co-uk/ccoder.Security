// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class ForgotPasswordController(
    IAuthenticationOrchestrationService authenticationOrchestrationService)
        : Controller
{
    [HttpPost("ForgotPassword")]
    public async ValueTask<IActionResult> PostForgotPassword(
        [FromBody] ForgotPasswordRequest newForgotPasswordRequest)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        try
        {
            await authenticationOrchestrationService.ForgotPasswordAsync(
                email: newForgotPasswordRequest.Email);
        }
        catch
        {
        }

        return Ok();
    }
}