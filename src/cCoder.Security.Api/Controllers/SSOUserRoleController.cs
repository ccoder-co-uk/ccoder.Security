using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Security.Api.Controllers;

public class SSOUserRoleController(
    ISSOUserRoleProcessingService ssoUserRoleProcessingService)
        : SecurityController<SSOUserRole>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUserRole> queryOptions) =>
        Ok(ssoUserRoleProcessingService.GetAllSSOUserRoles());
}