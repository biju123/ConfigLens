using System.Globalization;
using System.Text.RegularExpressions;

namespace ConfigLens.Domain.Scan;

/// <summary>
/// Identifies a completed or in-progress scan, formatted as SCAN-yyyyMMdd-NNNNN
/// (a per-day sequence number, e.g. SCAN-20260912-00124).
/// </summary>
public readonly partial record struct ScanId
{
    private const string Prefix = "SCAN";

    public DateOnly Date { get; }
    public int Sequence { get; }

    private ScanId(DateOnly date, int sequence)
    {
        if (sequence < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence), sequence, "Scan sequence must be positive.");
        }

        Date = date;
        Sequence = sequence;
    }

    public static ScanId Create(DateOnly date, int sequence) => new(date, sequence);

    public static ScanId Parse(string value)
    {
        if (!TryParse(value, out var scanId))
        {
            throw new FormatException($"'{value}' is not a valid ScanId. Expected format {Prefix}-yyyyMMdd-NNNNN.");
        }

        return scanId;
    }

    public static bool TryParse(string? value, out ScanId scanId)
    {
        scanId = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var match = ScanIdPattern().Match(value);
        if (!match.Success)
        {
            return false;
        }

        if (!DateOnly.TryParseExact(match.Groups["date"].Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return false;
        }

        var sequence = int.Parse(match.Groups["sequence"].Value, CultureInfo.InvariantCulture);
        if (sequence < 1)
        {
            return false;
        }

        scanId = new ScanId(date, sequence);
        return true;
    }

    public override string ToString() =>
        $"{Prefix}-{Date:yyyyMMdd}-{Sequence.ToString("D5", CultureInfo.InvariantCulture)}";

    [GeneratedRegex(@"^SCAN-(?<date>\d{8})-(?<sequence>\d{5,})$")]
    private static partial Regex ScanIdPattern();
}
