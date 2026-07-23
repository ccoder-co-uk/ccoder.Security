// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Exposures.Controllers;

public class SSOUserController(ISSOUserOrchestrationService ssoUserOrchestrationService)
        : SecurityController<SSOUser>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUser> queryOptions) =>
        Ok(value: ssoUserOrchestrationService.GetAllSSOUsers());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<SSOUser> result = ssoUserOrchestrationService
            .GetAllSSOUsers()
            .Where(predicate: i => i.Id == key);

        return result.Any()
            ? Ok(value: SingleResult.Create(queryable: result))
            : NotFound();
    }

    [HttpPut]
    [EnableQuery]
    public virtual async ValueTask<IActionResult> Put([FromRoute] string key, [FromBody] SSOUser ssoUser) =>
        ModelState.IsValid
            ? Get(key: (await ssoUserOrchestrationService.UpdateSSOUserAsync(username: key, item: ssoUser)).Id)
            : BadRequest(modelState: ModelState);

    [HttpDelete]
    public virtual async ValueTask<IActionResult> Delete([FromRoute] string key, string reference = null)
    {
        SSOUser origentity = ssoUserOrchestrationService
            .GetAllSSOUsers()
            .FirstOrDefault(predicate: i => i.Id == key);

        if (origentity == null)
        { return NotFound(); }

        await ssoUserOrchestrationService.DeleteSSOUserAsync(item: origentity);
        return Ok();
    }
}