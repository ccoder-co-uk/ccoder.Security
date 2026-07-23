// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class ForgotPasswordController(
    IAuthenticationAggregationService authenticationAggregationService)
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
            await authenticationAggregationService.ForgotPasswordAsync(
                email: newForgotPasswordRequest.Email);
        }
        catch
        {
        }

        return Ok();
    }
}