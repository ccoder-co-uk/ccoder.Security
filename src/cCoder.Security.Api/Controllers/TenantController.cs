using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class TenantController(ITenantProcessingService tenantProcessingService) 
    : SecurityController<Tenant>
{
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
}
