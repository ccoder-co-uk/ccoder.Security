// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Security.Web.Exposures;

namespace Security.Web.Controllers;

[Route("CurrentUser")]
public class CurrentUserController(
    ICurrentUserManager currentUserManager)
        : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(value: currentUserManager.GetCurrentUserId());
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The current user operation failed.");
        }
    }
}