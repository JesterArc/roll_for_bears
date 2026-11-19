using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using RollForBears.Modules.Shared_Kernel.Exceptions;

namespace RollForBears.Modules.Shared_Kernel.JsonConverters;

public class CustomJsonDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss zzz";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var datetime = reader.GetString()!;
        
        return DateTime.TryParseExact(datetime, DateTimeFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var date) 
            ? date : throw new InvalidFormatException($"Not a valid datetime format - use {DateTimeFormat}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateTimeFormat));
    }
}