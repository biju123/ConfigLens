using ConfigLens.Application.Comparison;
using ConfigLens.Domain.Configuration;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Application.Tests.Comparison;

public class AppConfigComparerTests
{
    [Fact]
    public void Detects_added_removed_and_changed_entries()
    {
        var current = new ConfigTree("AssessorApi", [
            new ConfigSection("Database", [
                new ConfigItem("CommandTimeout", "45", false),
                new ConfigItem("Provider", "AzureSql", false)
            ], [])
        ]);

        var baseline = new ConfigTree("AssessorApi", [
            new ConfigSection("Database", [
                new ConfigItem("CommandTimeout", "30", false),
                new ConfigItem("RetryCount", "3", false)
            ], [])
        ]);

        var result = AppConfigComparer.Compare(current, baseline);

        result.Added.Should().ContainSingle(e => e.Path == "Database.Provider");
        result.Removed.Should().ContainSingle(e => e.Path == "Database.RetryCount");
        result.Changed.Should().ContainSingle(c => c.Path == "Database.CommandTimeout" && c.OldValue == "30" && c.NewValue == "45");
    }

    [Fact]
    public void Counts_unchanged_entries()
    {
        var tree = new ConfigTree("AssessorApi", [
            new ConfigSection("Database", [new ConfigItem("CommandTimeout", "30", false)], [])
        ]);

        var result = AppConfigComparer.Compare(tree, tree);

        result.UnchangedCount.Should().Be(1);
        result.Added.Should().BeEmpty();
        result.Removed.Should().BeEmpty();
        result.Changed.Should().BeEmpty();
    }
}
