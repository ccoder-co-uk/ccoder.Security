using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Security.Api.Interfaces;
using Security.Objects.Entities;
using Security.Services.Orchestration.Interfaces;

namespace Security.Api.Controllers
{
    public class SSOUserController : ODataController
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
        public virtual async ValueTask<IActionResult> Put([FromRoute] string key, [FromBody] SSOUser ssoUser)
            => ModelState.IsValid
                ? Get((await ssoUserOrchestrationService.UpdateSSOUserAsync(key, ssoUser)).Id)
                : BadRequest(ModelState);

        [AcceptVerbs("PATCH", "MERGE")]
        public async ValueTask<IActionResult> Patch([FromRoute] string key, Delta<SSOUser> delta)
        {
            // don't like this, would rather pass the delta to the service layer but that would force a 
            // dependency on the OData framework in the business layer.
            SSOUser origentity = ssoUserOrchestrationService.GetAllSSOUsers().FirstOrDefault(i => i.Id == key);

            if (origentity == null)
                return NotFound();

            delta.Patch(origentity);
            return Ok(await ssoUserOrchestrationService.UpdateSSOUserAsync(key, origentity));
        }

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
}