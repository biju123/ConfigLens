using ConfigLens.Application.Scans.Aks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/scans/aks-deployment")]
[Authorize]
public sealed class AksDeploymentScanController(AksDeploymentScanService scanService) : ControllerBase
{
    [HttpPost]
    public ActionResult<AksScanResponse> Scan(AksScanRequest request) =>
        Ok(scanService.Execute(request, User.Identity?.Name ?? "unknown"));
}
