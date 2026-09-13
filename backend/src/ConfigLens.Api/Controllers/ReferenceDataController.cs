using ConfigLens.Domain.Reference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/reference-data")]
[Authorize]
public sealed class ReferenceDataController(IReferenceDataProvider referenceDataProvider) : ControllerBase
{
    [HttpGet]
    public ActionResult<ReferenceData> Get() => Ok(referenceDataProvider.GetReferenceData());
}
