using System;
using Microsoft.AspNetCore.Mvc;
using Security.Api.Interfaces;
using Security.Objects.DTOs;
using System.Threading.Tasks;

namespace Security.Api.Controllers
{
    [Route("Api/Account")]
    public class AccountController : Controller
    {
        private readonly IAccountManager accountManager;

        public AccountController(IAccountManager accountManager) =>
            this.accountManager = accountManager;

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
                catch (Exception ex)
                {

                }
                
                return Ok();
            }
            
            return BadRequest(ModelState);
        }

    }
}