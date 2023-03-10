using Cassandra;
using Confluent.Kafka;
using Nest;
using StreamProcessing.Consumer.DataPersistence;
using StreamProcessing.Consumer.DataPersistence.AggregatedStorage;
using StreamProcessing.Consumer.DataPersistence.PrimaryStorage;
using StreamProcessing.Consumer.EventReading;
using StreamProcessing.Contracts;

var cts = new CancellationTokenSource();
var cassandraCluster = Cluster.Builder()
    .AddContactPoints("localhost")
    .Build();
var elasticClient =
    new ElasticClient(
        new ConnectionSettings()
            .DefaultMappingFor<TurbineDailyData>(
                m => 
                    m.IndexName(ElasticSearchAggregatedDataWritingStrategy.Index)));
var dataWriters = new IDataWritingStrategy[]
{
    new CassandraRawDataWritingStrategy("streamprocessing", cassandraCluster),
    new ElasticSearchAggregatedDataWritingStrategy(elasticClient)
};
using var consumer = new KafkaConsumer(new[] { "localhost:29092" }, "scada-stream");

Console.WriteLine(@"Running consumer - this will simulate the process of receiving data belonging to one wind turbine.");

await consumer.Consume(ReceiveMessage, cts);

foreach (var writer in dataWriters)
{
    await writer.Flush();
    
    writer.Dispose();
}

Console.WriteLine(@"Consumer finished, press any key to exit...");
Console.ReadKey();

async Task ReceiveMessage(IConsumer<string, ScadaDataPoint> nativeConsumer, CancellationTokenSource cancellationTokenSource)
{
    var cr = nativeConsumer.Consume(cancellationTokenSource.Token);
    var dataPoint = cr.Message.Value;

    foreach (var writer in dataWriters)
        await writer.Write(dataPoint);
}