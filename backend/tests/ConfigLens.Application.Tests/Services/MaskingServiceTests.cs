using ConfigLens.Application.Services;
using ConfigLens.Domain.Configuration;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Application.Tests.Services;

public class MaskingServiceTests
{
    private readonly MaskingService _sut = new();

    [Theory]
    [InlineData("Password")]
    [InlineData("ConnectionString")]
    [InlineData("ApiKey")]
    [InlineData("api-key")]
    [InlineData("AccessToken")]
    [InlineData("ClientSecret")]
    [InlineData("Credential")]
    public void IsSensitiveKey_matches_known_sensitive_keywords_case_insensitively(string key) =>
        _sut.IsSensitiveKey(key).Should().BeTrue();

    [Fact]
    public void IsSensitiveKey_returns_false_for_ordinary_keys() =>
        _sut.IsSensitiveKey("CommandTimeout").Should().BeFalse();

    [Fact]
    public void MaskIfSensitive_replaces_the_value_and_flags_sensitive()
    {
        var masked = _sut.MaskIfSensitive(new ConfigItem("ConnectionString", "Server=x;Password=y;", false));

        masked.Value.Should().Be("********");
        masked.IsSensitive.Should().BeTrue();
    }

    [Fact]
    public void MaskIfSensitive_leaves_non_sensitive_items_untouched()
    {
        var item = new ConfigItem("RetryCount", "3", false);

        _sut.MaskIfSensitive(item).Should().Be(item);
    }

    [Fact]
    public void MaskTree_masks_recursively_through_subsections()
    {
        var tree = new ConfigTree("Assessor", [
            new ConfigSection("Database", [new ConfigItem("ConnectionString", "secret", false)], [
                new ConfigSection("Nested", [new ConfigItem("Token", "abc", false)], [])
            ])
        ]);

        var masked = _sut.MaskTree(tree);

        masked.Sections[0].Items[0].Value.Should().Be("********");
        masked.Sections[0].SubSections[0].Items[0].Value.Should().Be("********");
    }
}
