using cCoder.Security.Objects;
using Microsoft.AspNetCore.Mvc;

namespace SecuritySQLite.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly ISSOAuthInfo authInfo;

    public HomeController(ISSOAuthInfo authInfo)
    {
        this.authInfo = authInfo;
    }

    [HttpGet]
    public IActionResult Get() =>
        Ok(authInfo.SSOUserId);
}
