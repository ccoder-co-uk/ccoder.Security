using cCoder.Core.Objects.Dtos.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Api.Controllers
{
    public class SecurityController<T> : Controller
    {
        [HttpGet]
        public virtual IActionResult GetMetadata() =>
            Ok(GetMetadataForType(typeof(T), true, true));

        MetadataContainer GetMetadataForType(Type type, bool hasEndpoint, bool isEntity) =>
            new(type, isEntity, hasEndpoint);
    }
}