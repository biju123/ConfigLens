using ConfigLens.Application.Scans.AppConfig;
using ConfigLens.Application.Services;
using ConfigLens.Application.Tests.Fakes;
using ConfigLens.Domain.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigLens.Application.Tests.Scans;

public class AppConfigurationScanServiceTests
{
    [Fact]
    public void Execute_masks_sensitive_values_before_storing_or_returning()
    {
        var client = new FakeApplicationConfigurationClient().With("AssessorApi", "Production", "ContosoCounty", 2026,
            new ConfigTree("AssessorApi", [new ConfigSection("Database", [new ConfigItem("ConnectionString", "Server=x;Password=y;", false)], [])]));

        var repository = new InMemoryScanRepository();
        var sut = new AppConfigurationScanService(client, new MaskingService(), new ScanIdGenerator(new FakeClock(DateTime.UtcNow)), repository, new FakeClock(DateTime.UtcNow), NullLogger<AppConfigurationScanService>.Instance);

        var response = sut.Execute(new AppConfigScanRequest("AssessorApi", "Production", "ContosoCounty", null, 2026), "user");

        response.Tree.Sections[0].Items[0].Value.Should().Be("********");
        response.Tree.Sections[0].Items[0].IsSensitive.Should().BeTrue();
    }

    [Fact]
    public void Execute_returns_an_empty_tree_when_no_configuration_is_published()
    {
        var client = new FakeApplicationConfigurationClient();
        var repository = new InMemoryScanRepository();
        var sut = new AppConfigurationScanService(client, new MaskingService(), new ScanIdGenerator(new FakeClock(DateTime.UtcNow)), repository, new FakeClock(DateTime.UtcNow), NullLogger<AppConfigurationScanService>.Instance);

        var response = sut.Execute(new AppConfigScanRequest("AssessorApi", "Production", "ContosoCounty", null, 2024), "user");

        response.Tree.Sections.Should().BeEmpty();
    }
}
