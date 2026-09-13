namespace ConfigLens.Domain.Configuration;

/// <summary>
/// A single configuration key/value. IsSensitive reflects whether Value has
/// already been masked (see Application.Services.IMaskingService) - by the
/// time a ConfigItem leaves the Application layer for the API, any sensitive
/// value has been replaced with a fixed mask string.
/// </summary>
public sealed record ConfigItem(string Key, string? Value, bool IsSensitive);
