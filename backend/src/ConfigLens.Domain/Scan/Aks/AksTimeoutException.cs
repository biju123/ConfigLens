namespace ConfigLens.Domain.Scan.Aks;

/// <summary>A call to Azure Resource Manager or the AKS cluster's API server did not complete in time (CLAUDE.md section 23: timeout).</summary>
public sealed class AksTimeoutException(string message, Exception? innerException = null)
    : Exception(message, innerException);
