using ConfigLens.Application.Scans.AppConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/scans/application-configuration")]
[Authorize]
public sealed class ApplicationConfigurationScanController(AppConfigurationScanService scanService) : ControllerBase
{
    [HttpPost]
    public ActionResult<AppConfigScanResponse> Scan(AppConfigScanRequest request) =>
        Ok(scanService.Execute(request, User.Identity?.Name ?? "unknown"));
}
