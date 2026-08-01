// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Security.Web.Exposures;

namespace Security.Web.Controllers;

[Route("")]
public class HomeController(IHomeManager homeManager) : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return PhysicalFile(
                physicalPath: homeManager.GetIndexPath(),
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