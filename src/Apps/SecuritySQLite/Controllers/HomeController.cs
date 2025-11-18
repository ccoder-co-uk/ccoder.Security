using cCoder.Security.Objects;
using Microsoft.AspNetCore.Mvc;

namespace SecuritySQLite.Controllers;

[Route("")]
public class HomeController(ISSOAuthInfo authInfo) : Controller
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(authInfo.SSOUserId);
}
