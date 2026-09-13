using ConfigLens.Domain.Scan;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Domain.Tests.Scan;

public class ScanIdTests
{
    [Fact]
    public void ToString_formats_as_SCAN_yyyyMMdd_NNNNN() =>
        ScanId.Create(new DateOnly(2026, 9, 12), 124).ToString().Should().Be("SCAN-20260912-00124");

    [Fact]
    public void Parse_round_trips_ToString_output()
    {
        var original = ScanId.Create(new DateOnly(2026, 9, 12), 124);

        ScanId.Parse(original.ToString()).Should().Be(original);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-scan-id")]
    [InlineData("SCAN-2026912-00124")]
    [InlineData("SCAN-20260912-0")]
    [InlineData(null)]
    public void TryParse_rejects_malformed_input(string? value) =>
        ScanId.TryParse(value, out _).Should().BeFalse();

    [Fact]
    public void Create_rejects_a_non_positive_sequence() =>
        FluentActions.Invoking(() => ScanId.Create(new DateOnly(2026, 9, 12), 0))
            .Should().Throw<ArgumentOutOfRangeException>();

    [Fact]
    public void Equality_is_value_based()
    {
        var a = ScanId.Create(new DateOnly(2026, 9, 12), 1);
        var b = ScanId.Create(new DateOnly(2026, 9, 12), 1);

        (a == b).Should().BeTrue();
    }
}
