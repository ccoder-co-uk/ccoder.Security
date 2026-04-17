using System.Security;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Api.Controllers;

public class SSOUserController(
    IAccountManager accountManager,
    IServiceProvider serviceProvider)
        : SecurityController<SSOUser>
{
    private ISSOUserOrchestrationService SsoUserOrchestrationService =>
        serviceProvider.GetRequiredService<ISSOUserOrchestrationService>();

    [HttpGet]
    public IActionResult Me() =>
        Ok(accountManager.Me());

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUser> queryOptions) =>
        Ok(SsoUserOrchestrationService.GetAllSSOUsers());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<SSOUser> result = SsoUserOrchestrationService
            .GetAllSSOUsers()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    [HttpPut]
    [EnableQuery]
    public virtual async ValueTask<IActionResult> Put([FromRoute] string key, [FromBody] SSOUser ssoUser) =>
        ModelState.IsValid
            ? Get((await SsoUserOrchestrationService.UpdateSSOUserAsync(key, ssoUser)).Id)
            : BadRequest(ModelState);

    [HttpDelete]
    public virtual async ValueTask<IActionResult> Delete([FromRoute] string key, string reference = null)
    {
        SSOUser origentity = SsoUserOrchestrationService
            .GetAllSSOUsers()
            .FirstOrDefault(i => i.Id == key);

        if (origentity == null)
            return NotFound();

        await SsoUserOrchestrationService.DeleteSSOUserAsync(origentity);
        return Ok();
    }

    [HttpPost("AcceptInvite")]
    public virtual async ValueTask<IActionResult> AcceptInvite([FromQuery] string userId, [FromQuery] string inviteToken, [FromBody] RegisterUser inviteForm)
    {
        await accountManager.AcceptInviteAsync(inviteForm, userId, inviteToken);

        return Ok();
    }
}

