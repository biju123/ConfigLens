using ConfigLens.Application.Scans.Characteristics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/scans/configuration-characteristics")]
[Authorize]
public sealed class ConfigurationCharacteristicsScanController(CharacteristicsScanService scanService) : ControllerBase
{
    [HttpPost]
    public ActionResult<CharacteristicsScanResponse> Scan(CharacteristicsScanRequest request) =>
        Ok(scanService.Execute(request, User.Identity?.Name ?? "unknown"));
}
