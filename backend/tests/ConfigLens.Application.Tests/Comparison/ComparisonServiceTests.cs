using ConfigLens.Application.Comparison;
using ConfigLens.Application.Services;
using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Aks;
using ConfigLens.Domain.Scan.Dependency;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Application.Tests.Comparison;

public class ComparisonServiceTests
{
    private static ScanRecord MakeAksRecord(ScanId scanId) => new(
        new ScanMetadata
        {
            ScanId = scanId,
            Category = ScanCategory.AksDeployment,
            StartedAtUtc = DateTime.UtcNow,
            Status = ScanStatus.Completed,
            InitiatingUser = "user"
        },
        new AksScanResult([]));

    private static ScanRecord MakeDependencyRecord(ScanId scanId) => new(
        new ScanMetadata
        {
            ScanId = scanId,
            Category = ScanCategory.DependencyAccessibility,
            StartedAtUtc = DateTime.UtcNow,
            Status = ScanStatus.Completed,
            InitiatingUser = "user"
        },
        new DependencyScanResult([]));

    [Fact]
    public void Compare_throws_CategoryMismatchException_for_different_categories()
    {
        var repository = new InMemoryScanRepository();
        var current = ScanId.Create(new DateOnly(2026, 9, 12), 1);
        var baseline = ScanId.Create(new DateOnly(2026, 9, 11), 1);
        repository.Add(MakeAksRecord(current));
        repository.Add(MakeDependencyRecord(baseline));

        var sut = new ComparisonService(repository);
        var act = () => sut.Compare(new CompareScansRequest(current.ToString(), baseline.ToString()));

        act.Should().Throw<CategoryMismatchException>();
    }

    [Fact]
    public void Compare_throws_ScanNotFoundException_when_a_scan_id_is_unknown()
    {
        var repository = new InMemoryScanRepository();
        var current = ScanId.Create(new DateOnly(2026, 9, 12), 1);
        repository.Add(MakeAksRecord(current));

        var sut = new ComparisonService(repository);
        var unknown = ScanId.Create(new DateOnly(2026, 9, 1), 99);
        var act = () => sut.Compare(new CompareScansRequest(current.ToString(), unknown.ToString()));

        act.Should().Throw<ScanNotFoundException>();
    }

    [Fact]
    public void Compare_dispatches_to_the_matching_categorys_comparer()
    {
        var repository = new InMemoryScanRepository();
        var current = ScanId.Create(new DateOnly(2026, 9, 12), 1);
        var baseline = ScanId.Create(new DateOnly(2026, 9, 11), 1);
        repository.Add(MakeAksRecord(current));
        repository.Add(MakeAksRecord(baseline));

        var sut = new ComparisonService(repository);
        var response = sut.Compare(new CompareScansRequest(current.ToString(), baseline.ToString()));

        response.Category.Should().Be(ScanCategory.AksDeployment);
        response.Result.Should().BeOfType<AksComparisonResult>();
    }
}
