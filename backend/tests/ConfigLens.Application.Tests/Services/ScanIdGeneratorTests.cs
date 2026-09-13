using ConfigLens.Application.Services;
using ConfigLens.Application.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Application.Tests.Services;

public class ScanIdGeneratorTests
{
    [Fact]
    public void Next_formats_as_SCAN_yyyyMMdd_NNNNN()
    {
        var clock = new FakeClock(new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc));
        var sut = new ScanIdGenerator(clock);

        sut.Next().ToString().Should().Be("SCAN-20260912-00001");
    }

    [Fact]
    public void Next_increments_the_sequence_within_the_same_day()
    {
        var clock = new FakeClock(new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc));
        var sut = new ScanIdGenerator(clock);

        sut.Next();
        sut.Next();
        var third = sut.Next();

        third.ToString().Should().Be("SCAN-20260912-00003");
    }

    [Fact]
    public void Next_resets_the_sequence_on_a_new_day()
    {
        var clock = new FakeClock(new DateTime(2026, 9, 12, 23, 59, 0, DateTimeKind.Utc));
        var sut = new ScanIdGenerator(clock);
        sut.Next();

        clock.UtcNow = new DateTime(2026, 9, 13, 0, 1, 0, DateTimeKind.Utc);
        var next = sut.Next();

        next.ToString().Should().Be("SCAN-20260913-00001");
    }
}
