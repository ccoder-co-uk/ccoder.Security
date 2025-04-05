using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class SSORoleController(
    ISSORoleService ssoRoleService) 
        : SecurityController<SSORole>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSORole> queryOptions) =>
        Ok(ssoRoleService.GetAllSSORoles());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<SSORole> result = ssoRoleService
            .GetAllSSORoles()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    public virtual async ValueTask<IActionResult> Post([FromBody] SSORole role) =>
        Ok(ssoRoleService.AddSSORoleAsync(role));

    public virtual async ValueTask<IActionResult> Put([FromRoute] Guid key, [FromBody] SSORole role) =>
        Ok(ssoRoleService.UpdateSSORoleAsync(role));
}