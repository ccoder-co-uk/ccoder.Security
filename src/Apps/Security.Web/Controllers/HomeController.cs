// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using cCoder.Security.Brokers.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security.Web.Services.Foundations;

namespace Security.Web.Controllers;

[Route("")]
public class HomeController(
    IHomeService homeService,
    ILoggingBroker loggingBroker)
        : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            PhysicalFileResult result = PhysicalFile(
                physicalPath: homeService.GetIndexPath(),
                contentType: "text/html");

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return result;
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The home page operation failed.");
        }
    }
}