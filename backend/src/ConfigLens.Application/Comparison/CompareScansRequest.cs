namespace ConfigLens.Application.Comparison;

public sealed record CompareScansRequest(string CurrentScanId, string BaselineScanId);
