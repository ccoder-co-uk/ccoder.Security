using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class SSOPrivilegeController(ISSOPrivilegeProcessingService ssoPrivilegeProcessingService) 
    : SecurityController<SSOPrivilege>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOPrivilege> queryOptions) =>
        Ok(ssoPrivilegeProcessingService.GetAllSSOPrivileges());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<SSOPrivilege> result = ssoPrivilegeProcessingService
            .GetAllSSOPrivileges()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }
}
