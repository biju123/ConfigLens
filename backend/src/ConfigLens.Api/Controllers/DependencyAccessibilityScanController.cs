using ConfigLens.Application.Scans.Dependency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/scans/dependency-accessibility")]
[Authorize]
public sealed class DependencyAccessibilityScanController(DependencyAccessibilityScanService scanService) : ControllerBase
{
    [HttpPost]
    public ActionResult<DependencyScanResponse> Scan(DependencyScanRequest request) =>
        Ok(scanService.Execute(request, User.Identity?.Name ?? "unknown"));
}
