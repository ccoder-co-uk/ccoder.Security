// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Security.Web.Services.Foundations;

namespace Security.Web.Controllers;

[Route("CurrentUser")]
public class CurrentUserController(
    ICurrentUserService currentUserService)
        : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(value: currentUserService.GetCurrentUserId());
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The current user operation failed.");
        }
    }
}