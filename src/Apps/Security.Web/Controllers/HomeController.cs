// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Security.Web.Services.Foundations;

namespace Security.Web.Controllers;

[Route("")]
public class HomeController(IHomeService homeService) : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return PhysicalFile(
                physicalPath: homeService.GetIndexPath(),
                contentType: "text/html");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The home page operation failed.");
        }
    }
}