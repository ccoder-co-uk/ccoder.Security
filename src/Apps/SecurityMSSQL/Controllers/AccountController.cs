using cCoder.Security.Exposures;
using cCoder.Security.Objects.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SecurityMSSQL.Controllers;

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
    public async ValueTask<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await accountManager.ForgotPasswordAsync(request.Email);
            }
            catch
            {
                // Deliberate, don't indicate any sort of problem as this could be used against us.
            }

            return Ok();
        }

        return BadRequest(ModelState);
    }
}