namespace ConfigLens.Domain.Scan.Aks;

/// <summary>The requested Azure subscription or AKS cluster does not exist or is not accessible to the caller.</summary>
public sealed class AksClusterNotFoundException(string message, Exception? innerException = null)
    : Exception(message, innerException);
