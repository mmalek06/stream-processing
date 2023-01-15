using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using StreamProcessing.Contracts;

namespace StreamProcessing.Producer;

public class ScadaDataPointSerializer : ISerializer<ScadaDataPoint>
{
    public byte[] Serialize(ScadaDataPoint data, SerializationContext context) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
}