// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Security.Web.Controllers;

[Route("")]
public class HomeController(ISSOAuthInfo authInfo, IWebHostEnvironment environment) : Controller
{
    [HttpGet]
    public IActionResult Get() =>
        PhysicalFile(
physicalPath: Path.Combine(environment.WebRootPath, "index.html"),
contentType: "text/html");

    [HttpGet("CurrentUser")]
    public IActionResult CurrentUser() =>
        Ok(value: authInfo.SSOUserId);
}