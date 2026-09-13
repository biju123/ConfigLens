namespace ConfigLens.Domain.Configuration;

/// <summary>The full configuration returned by an application's configuration-inspection API, rooted at the application name (e.g. "Assessor").</summary>
public sealed record ConfigTree(string RootName, IReadOnlyList<ConfigSection> Sections);
