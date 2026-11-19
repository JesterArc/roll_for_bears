using System.Text.Json;
using System.Text.Json.Serialization;
using RollForBears.Modules.Shared_Kernel.Exceptions;

namespace RollForBears.Modules.Shared_Kernel.JsonConverters;

public class CustomJsonGuidConverter : JsonConverter<Guid>
{
    private const string GuidFormat = "D";
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var guid = reader.GetString()!;
        
        return Guid.TryParseExact(guid, GuidFormat, out var result) 
            ? result : throw new InvalidFormatException("Invalid identifier format");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(GuidFormat));
    }
}