using cCoder.Security.Api.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc;

namespace SecuritySQLite.Controllers;

[Route("Api/Account")]
public class RegisterController : Controller
{
    private readonly IAccountManager accountManager;

    public RegisterController(IAccountManager accountManager)
    {
        this.accountManager = accountManager;
    }

    [HttpPost("Register")]
    public async ValueTask<IActionResult> Register([FromBody] RegisterUser registerForm)
    {
        if (!ModelState.IsValid)
            BadRequest(ModelState);

        (SSOUser user, string confirmationToken) = await accountManager.RegisterAsync(registerForm);
        var result = new
        {
            User = user,
            Token = confirmationToken
        };

        return Ok(result);
    }

    [HttpPost("ConfirmRegistration")]
    public async ValueTask<IActionResult> ConfirmRegistration(string confirmationToken)
    {
        await accountManager.ConfirmRegistrationAsync(confirmationToken);
        return Ok();
    }
}