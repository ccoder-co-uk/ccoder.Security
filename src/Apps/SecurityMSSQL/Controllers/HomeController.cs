using cCoder.Security.Objects;
using Microsoft.AspNetCore.Mvc;

namespace SecurityMSSQL.Controllers;

[Route("")]
public class HomeController(ISSOAuthInfo authInfo) : Controller
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(authInfo.SSOUserId);
}
