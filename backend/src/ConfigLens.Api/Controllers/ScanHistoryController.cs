using ConfigLens.Domain.Scan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

[ApiController]
[Route("api/scans")]
[Authorize]
public sealed class ScanHistoryController(IScanRepository scanRepository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<ScanRecord>> Query([FromQuery] ScanCategory? category) =>
        Ok(scanRepository.Query(category));

    [HttpGet("{scanId}")]
    public ActionResult<ScanRecord> GetById(string scanId)
    {
        if (!ScanId.TryParse(scanId, out var id))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, type: "invalid-input", title: $"'{scanId}' is not a valid scan id.");
        }

        var record = scanRepository.GetById(id);
        return record is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, type: "not-found", title: $"Scan '{scanId}' was not found.")
            : Ok(record);
    }
}
