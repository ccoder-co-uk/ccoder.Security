// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using cCoder.Security.Brokers.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security.Web.Services.Foundations;

namespace Security.Web.Controllers;

[Route("CurrentUser")]
public class CurrentUserController(
    ICurrentUserService currentUserService,
    ILoggingBroker loggingBroker)
        : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            string userId = currentUserService.GetCurrentUserId();
            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return Ok(value: userId);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The current user operation failed.");
        }
    }
}