// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class AuthenticationController(
    IAuthenticationManager authenticationAggregationService)
        : Controller
{
    [HttpPost("Login")]
    public async ValueTask<IActionResult> PostLogin([FromBody] Auth newAuth) =>
        ModelState.IsValid
            ? Ok(value: await authenticationAggregationService.LoginAsync(
                username: newAuth.User,
                password: newAuth.Pass))
            : BadRequest(modelState: ModelState);

    [HttpPost("Logout")]
    public async ValueTask<IActionResult> PostLogout()
    {
        await authenticationAggregationService.LogoutAsync();
        return Ok();
    }
}