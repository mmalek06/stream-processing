using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer;

public class ScadaDataPointDeserializer : IDeserializer<ScadaDataPoint>
{
    public ScadaDataPoint Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) =>
        JsonSerializer.Deserialize<ScadaDataPoint>(Encoding.UTF8.GetString(data))!;
}