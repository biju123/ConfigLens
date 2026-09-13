using ConfigLens.Application.Comparison;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/comparisons")]
[Authorize]
public sealed class ComparisonController(IComparisonService comparisonService) : ControllerBase
{
    [HttpPost]
    public ActionResult<ComparisonResponse> Compare(CompareScansRequest request) =>
        Ok(comparisonService.Compare(request));
}
