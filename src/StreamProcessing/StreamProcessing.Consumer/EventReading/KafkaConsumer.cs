using Confluent.Kafka;
using Confluent.Kafka.Admin;
using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer.EventReading;

public class KafkaConsumer : IDisposable
{
    private const int InformEveryNLines = 1000;
    private const int KillConsumerDelaySeconds = 20;

    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly ConsumerConfig _consumerConfig;
    
    private IConsumer<string, ScadaDataPoint>? _consumer;

    public KafkaConsumer(IEnumerable<string> bootstrapServers, string topic)
    {
        _bootstrapServers = string.Join(';', bootstrapServers);
        _topic = topic;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = Guid.NewGuid().ToString()
        };
    }

    public async Task Consume(Func<IConsumer<string, ScadaDataPoint>, CancellationTokenSource, Task> receiveMessage, CancellationTokenSource cts)
    {
        _consumer = new ConsumerBuilder<string, ScadaDataPoint>(_consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(new ScadaDataPointDeserializer())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();

        await CreateTopic();
        
        _consumer.Subscribe(_topic);

        var counter = 0;
        var lastMessageReceivedAt = DateTime.UtcNow;

        SetupConsumptionInterrupt(cts, lastMessageReceivedAt);
        
        try
        {
            while (true)
            {
                try
                {
                    await receiveMessage(_consumer, cts);
                    
                    lastMessageReceivedAt = DateTime.UtcNow;
                    counter++;

                    if (counter % InformEveryNLines == 0)
                        Console.WriteLine(@$"Imported {counter} lines so far.");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _consumer.Close();
        }
    }

    public void Dispose() =>
        _consumer?.Dispose();
    
    private async Task CreateTopic()
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build();
    
        try
        {
            await adminClient.CreateTopicsAsync(new[] { 
                new TopicSpecification { Name = _topic, ReplicationFactor = 1, NumPartitions = 1 } 
            });
        }
        catch (CreateTopicsException e)
        {
            Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }
    }
    
    private static void SetupConsumptionInterrupt(CancellationTokenSource cts, DateTime lastMessageReceivedAt)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                if ((DateTime.UtcNow - lastMessageReceivedAt).Seconds >= KillConsumerDelaySeconds)
                {
                    cts.Cancel();

                    break;
                }
                else
                    await Task.Delay(TimeSpan.FromSeconds(KillConsumerDelaySeconds / 2));
            }
        });
    }
}