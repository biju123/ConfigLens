using ConfigLens.Domain.Scan;

namespace ConfigLens.Domain.Comparison;

/// <summary>Thrown when a comparison is attempted between scans of different categories (CLAUDE.md section 13: "only compatible scans should be compared").</summary>
public sealed class CategoryMismatchException(ScanCategory currentCategory, ScanCategory baselineCategory)
    : InvalidOperationException(
        $"Cannot compare a {currentCategory} scan against a {baselineCategory} scan; both scans must be the same category.")
{
    public ScanCategory CurrentCategory { get; } = currentCategory;
    public ScanCategory BaselineCategory { get; } = baselineCategory;
}
