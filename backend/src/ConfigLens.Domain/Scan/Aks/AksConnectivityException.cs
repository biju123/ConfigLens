namespace ConfigLens.Domain.Scan.Aks;

/// <summary>Azure Resource Manager or the AKS cluster's API server could not be reached (CLAUDE.md section 23: infrastructure failure).</summary>
public sealed class AksConnectivityException(string message, Exception? innerException = null)
    : Exception(message, innerException);
