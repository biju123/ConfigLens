using System.Text.Json;
using System.Text.Json.Serialization;
using ConfigLens.Domain.Scan;

namespace ConfigLens.Api.Serialization;

/// <summary>Without this, System.Text.Json would serialize ScanId's Date/Sequence fields instead of the "SCAN-yyyyMMdd-NNNNN" string (CLAUDE.md section 12).</summary>
public sealed class ScanIdJsonConverter : JsonConverter<ScanId>
{
    public override ScanId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        ScanId.Parse(reader.GetString() ?? throw new JsonException("Expected a scan id string."));

    public override void Write(Utf8JsonWriter writer, ScanId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}
