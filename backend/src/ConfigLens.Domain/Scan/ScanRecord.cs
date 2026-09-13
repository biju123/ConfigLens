namespace ConfigLens.Domain.Scan;

public sealed record ScanRecord(ScanMetadata Metadata, IScanResult Result);
