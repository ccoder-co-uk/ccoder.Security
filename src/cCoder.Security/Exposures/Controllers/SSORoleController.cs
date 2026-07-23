// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Exposures.Controllers;

public class SSORoleController(ISSORoleOrchestrationService roleOrchestrationService)
    : SecurityController<SSORole>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSORole> queryOptions) =>
        Ok(value: roleOrchestrationService.GetAllSSORoles());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<SSORole> result = roleOrchestrationService
            .GetAllSSORoles()
            .Where(predicate: i => i.Id == key);

        return result.Any()
            ? Ok(value: SingleResult.Create(queryable: result))
            : NotFound();
    }

    [HttpPost]
    public virtual async ValueTask<IActionResult> Post([FromBody] SSORole newSSORole) =>
        Ok(value: await roleOrchestrationService.AddSSORoleAsync(item: newSSORole));

    [HttpPut]
    public virtual async ValueTask<IActionResult> Put(
        [FromRoute] Guid key,
        [FromBody] SSORole updatedSSORole) =>
        Ok(value: await roleOrchestrationService.UpdateSSORoleAsync(item: updatedSSORole));

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromRoute] Guid key)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        var role = roleOrchestrationService
            .GetAllSSORoles()
            .FirstOrDefault(predicate: r => r.Id == key);

        if (role is null)
        { return NotFound(); }

        await roleOrchestrationService.DeleteSSORoleAsync(item: role);

        return Ok();
    }
}