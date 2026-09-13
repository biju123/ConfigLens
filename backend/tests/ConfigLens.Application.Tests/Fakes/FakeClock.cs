using ConfigLens.Application.Services;

namespace ConfigLens.Application.Tests.Fakes;

public sealed class FakeClock(DateTime utcNow) : IClock
{
    public DateTime UtcNow { get; set; } = utcNow;
}
