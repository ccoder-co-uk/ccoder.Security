using cCoder.Security.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Security.Web.Controllers;

[Route("")]
public class HomeController(ISSOAuthInfo authInfo, IWebHostEnvironment environment) : Controller
{
    [HttpGet]
    public IActionResult Get() =>
        PhysicalFile(
            Path.Combine(environment.WebRootPath, "index.html"),
            "text/html");

    [HttpGet("CurrentUser")]
    public IActionResult CurrentUser() =>
        Ok(authInfo.SSOUserId);
}
