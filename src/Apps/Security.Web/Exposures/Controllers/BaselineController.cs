using Microsoft.AspNetCore.Mvc;
using Security.Web.Exposures.Setup;

namespace Security.Web.Exposures.Controllers;

[ApiController]
[Route("Api/Security/Baseline")]
public sealed class BaselineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(UIBaseline.Packages);
}
