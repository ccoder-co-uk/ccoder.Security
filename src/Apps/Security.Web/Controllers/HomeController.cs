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
    public IActionResult Get() =>
        PhysicalFile(
            physicalPath: homeManager.GetIndexPath(),
            contentType: "text/html");
}