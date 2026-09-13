using ConfigLens.Domain.Configuration;
using ConfigLens.Infrastructure.ApplicationApis;
using ConfigLens.SampleData;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Infrastructure.Tests;

public class SampleDataRuleSetProviderTests
{
    private readonly SampleDataRuleSetProvider _sut = new(new SampleDataProvider());

    [Theory]
    [InlineData("default")]
    [InlineData("strict")]
    public void GetRuleSet_loads_the_named_rule_set_with_at_least_one_rule(string ruleSetId)
    {
        var ruleSet = _sut.GetRuleSet(ruleSetId);

        ruleSet.Should().NotBeNull();
        ruleSet!.Rules.Should().NotBeEmpty();
    }

    [Fact]
    public void GetRuleSet_returns_null_for_an_unknown_id() =>
        _sut.GetRuleSet("nonexistent").Should().BeNull();

    [Fact]
    public void GetAllRuleSets_returns_both_sample_rule_sets() =>
        _sut.GetAllRuleSets().Select(r => r.RuleSetId).Should().BeEquivalentTo(["default", "strict"]);

    [Fact]
    public void Strict_rule_set_has_more_rules_than_default_and_a_tighter_range()
    {
        var defaultSet = _sut.GetRuleSet("default")!;
        var strictSet = _sut.GetRuleSet("strict")!;

        strictSet.Rules.Count.Should().BeGreaterThan(defaultSet.Rules.Count);

        var defaultRange = defaultSet.Rules.Single(r => r.RuleId == "database-commandtimeout-range");
        var strictRange = strictSet.Rules.Single(r => r.RuleId == "database-commandtimeout-range");
        strictRange.Parameters["max"].Should().NotBe(defaultRange.Parameters["max"]);
    }
}
