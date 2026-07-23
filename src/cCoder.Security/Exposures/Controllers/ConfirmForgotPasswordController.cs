// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class ConfirmForgotPasswordController(
    IAuthenticationAggregationService authenticationAggregationService)
        : Controller
{
    [HttpPost("ConfirmForgotPassword")]
    public async ValueTask<IActionResult> PostConfirmForgotPassword(
        [FromBody] ConfirmForgotPasswordRequest newConfirmForgotPasswordRequest)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        await authenticationAggregationService.ConfirmForgotPasswordAsync(
            tokenId: newConfirmForgotPasswordRequest.Token,
            userId: newConfirmForgotPasswordRequest.UserId,
            newPassword: newConfirmForgotPasswordRequest.NewPassword,
            confirmNewPassword: newConfirmForgotPasswordRequest.ConfirmPassword);

        return Ok();
    }
}