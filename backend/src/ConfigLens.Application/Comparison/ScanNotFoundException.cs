using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Comparison;

public sealed class ScanNotFoundException(ScanId scanId) : InvalidOperationException($"Scan '{scanId}' was not found.")
{
    public ScanId ScanId { get; } = scanId;
}
