using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class TenantController : Controller
{
    private readonly ITenantProcessingService tenantProcessingService;

    public TenantController(ITenantProcessingService tenantProcessingService)
    {
        this.tenantProcessingService = tenantProcessingService;
    }

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<Tenant> queryOptions) =>
        Ok(tenantProcessingService.GetAllTenants());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<Tenant> result = tenantProcessingService
            .GetAllTenants()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    [HttpPut]
    [EnableQuery]
    public virtual async ValueTask<IActionResult> Put([FromRoute] string key, [FromBody] Tenant ssoUser) => 
        ModelState.IsValid
            ? Get((await tenantProcessingService.UpdateTenantAsync(ssoUser)).Id)
            : BadRequest(ModelState);

    [HttpPost]
    public virtual async ValueTask<IActionResult> Post([FromBody] Tenant ssoUser) =>
        ModelState.IsValid
            ? Ok(await tenantProcessingService.AddTenantAsync(ssoUser))
            : BadRequest(ModelState);

    [HttpDelete]
    public virtual async ValueTask<IActionResult> Delete([FromRoute] string key, string reference = null)
    {
        Tenant origentity = tenantProcessingService
            .GetAllTenants()
            .FirstOrDefault(i => i.Id == key);

        if (origentity == null)
            return NotFound();

        await tenantProcessingService.DeleteTenantAsync(origentity);
        return Ok();
    }
}
