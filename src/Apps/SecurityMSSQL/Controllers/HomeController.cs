using Microsoft.AspNetCore.Mvc;
using cCoder.Security.Objects;

namespace cCoder.SecurityMSSQL.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ISSOAuthInfo authInfo;

        public HomeController(ISSOAuthInfo authInfo) =>
            this.authInfo = authInfo;

        [HttpGet]
        public IActionResult Get() =>
            Ok(authInfo.SSOUserId);
    }
}
