using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using cCoder.Security.Api.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;

namespace cCoder.Security.Api.Controllers;

public class SSOUserController : Controller
{
    private readonly IAccountManager accountManager;
    private readonly ISSOUserOrchestrationService ssoUserOrchestrationService;

    public SSOUserController(
        IAccountManager ssoUserProcessingService,
        ISSOUserOrchestrationService ssoUserOrchestrationService)
    {
        accountManager = ssoUserProcessingService;
        this.ssoUserOrchestrationService = ssoUserOrchestrationService;
    }

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUser> queryOptions) =>
        Ok(ssoUserOrchestrationService.GetAllSSOUsers());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<SSOUser> result = ssoUserOrchestrationService
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
            ? Get((await ssoUserOrchestrationService.UpdateSSOUserAsync(key, ssoUser)).Id)
            : BadRequest(ModelState);

    [HttpDelete]
    public virtual async ValueTask<IActionResult> Delete([FromRoute] string key, string reference = null)
    {
        SSOUser origentity = ssoUserOrchestrationService
            .GetAllSSOUsers()
            .FirstOrDefault(i => i.Id == key);

        if (origentity == null)
            return NotFound();

        await ssoUserOrchestrationService.DeleteSSOUserAsync(origentity);
        return Ok();
    }

    [HttpGet]
    public IActionResult Me() =>
        Ok(accountManager.Me());
}