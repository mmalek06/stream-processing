using Confluent.Kafka;
using StreamProcessing.Consumer;
using StreamProcessing.Contracts;

const string BootstrapServers = "localhost:29092";
const string Topic = "scada-stream";
var cts = new CancellationTokenSource();
var dataWriters = new IDataWritingStrategy[]
{
    new CassandraRawDataWritingStrategy(),
    new ElasticSearchAggregatedDataWritingStrategy()
};
using var consumer = new KafkaConsumer(new[] { BootstrapServers }, Topic);

Console.WriteLine(@"Running consumer - this will simulate the process of receiving data belonging to one wind turbine.");

await consumer.Consume(ReceiveMessage, cts);

Console.WriteLine(@"Consumer finished, press any key to exit...");
Console.ReadKey();

async Task ReceiveMessage(IConsumer<string, ScadaDataPoint> nativeConsumer, CancellationTokenSource cancellationTokenSource)
{
    var cr = nativeConsumer.Consume(cancellationTokenSource.Token);
    var dataPoint = cr.Message.Value;

    foreach (var writer in dataWriters)
        await writer.Write(dataPoint);
}