using ConfigLens.Domain.Scan.Aks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Controllers;

/// <summary>
/// Backs the AKS Deployment Scan parameter form: populating the "AKS
/// cluster" dropdown once a subscription id is entered, and the "Namespace"
/// dropdown once a cluster is selected (CLAUDE.md section 7).
/// </summary>
[ApiController]
[Route("api/aks")]
[Authorize]
public sealed class AksDiscoveryController(IAksClusterDirectory clusterDirectory) : ControllerBase
{
    [HttpGet("clusters")]
    public ActionResult<IReadOnlyList<string>> GetClusters([FromQuery] string subscriptionId)
    {
        if (string.IsNullOrWhiteSpace(subscriptionId))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, type: "invalid-input", title: "'subscriptionId' must not be empty.");
        }

        return Ok(clusterDirectory.GetClusters(subscriptionId));
    }

    [HttpGet("namespaces")]
    public ActionResult<IReadOnlyList<string>> GetNamespaces([FromQuery] string subscriptionId, [FromQuery] string clusterName)
    {
        if (string.IsNullOrWhiteSpace(subscriptionId))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, type: "invalid-input", title: "'subscriptionId' must not be empty.");
        }
        if (string.IsNullOrWhiteSpace(clusterName))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, type: "invalid-input", title: "'clusterName' must not be empty.");
        }

        return Ok(clusterDirectory.GetNamespaces(subscriptionId, clusterName));
    }
}
