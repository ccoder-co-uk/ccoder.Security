using cCoder.Security.Exposures;
using cCoder.Security.Objects.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SecuritySQLite.Controllers;

[Route("Api/Account")]
public class AccountController(IAccountManager accountManager) : Controller
{
    [HttpPost("Login")]
    public async ValueTask<IActionResult> Login([FromBody] Auth auth) =>
        ModelState.IsValid
            ? Ok(await accountManager.LoginAsync(auth.User, auth.Pass))
            : BadRequest(ModelState);

    [HttpPost("Logout")]
    public async ValueTask<IActionResult> Logout()
    {
        await accountManager.LogoutAsync();
        return Ok();
    }

    [HttpPost("ForgotPassword")]
    public async ValueTask<IActionResult> ChangePassword(string email, int appId)
    {
        if (ModelState.IsValid)
        {
            await accountManager.ForgotPasswordAsync(email);
            return Ok();
        }

        return BadRequest(ModelState);
    }
}