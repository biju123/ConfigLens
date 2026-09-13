using ConfigLens.Application.Scans.Dependency;
using ConfigLens.Application.Services;
using ConfigLens.Application.Tests.Fakes;
using ConfigLens.Domain.Scan.Dependency;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigLens.Application.Tests.Scans;

public class DependencyAccessibilityScanServiceTests
{
    [Fact]
    public void Execute_returns_checks_and_stores_the_scan()
    {
        var checks = new[]
        {
            new DependencyCheckResult { Application = "AssessorApi", DependencyType = DependencyType.AzureSql, TargetDescription = "t", Status = DependencyStatus.Accessible, CheckedAtUtc = DateTime.UtcNow }
        };
        var repository = new InMemoryScanRepository();
        var sut = new DependencyAccessibilityScanService(new FakeDependencyAccessibilityChecker(checks),
            new ScanIdGenerator(new FakeClock(DateTime.UtcNow)), repository, new FakeClock(DateTime.UtcNow), NullLogger<DependencyAccessibilityScanService>.Instance);

        var response = sut.Execute(new DependencyScanRequest("AssessorApi", "Production", "assessor-prod", null), "user");

        response.Checks.Should().BeEquivalentTo(checks);
        repository.GetById(response.Metadata.ScanId).Should().NotBeNull();
    }
}
